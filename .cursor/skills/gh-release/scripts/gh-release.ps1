<#
.SYNOPSIS
  Bump Unity bundleVersion (minor), build Android APK, create GitHub Release.

.PARAMETER Notes
  Release notes (required for a real release).

.PARAMETER Version
  Optional explicit version (e.g. 0.2.0). If omitted, prompts with proposed minor bump.

.PARAMETER SkipBuild
  Skip Unity build (use existing APK).

.PARAMETER ProjectRoot
  Repo root. Defaults to parent of this script's ../../../../ from skills path, or cwd.

.PARAMETER BuildTimeoutMinutes
  Max wait when asking an open Editor to build (default 45).
#>
param(
    [Parameter(Mandatory = $false)]
    [string]$Notes = "",

    [Parameter(Mandatory = $false)]
    [string]$Version = "",

    [switch]$SkipBuild,

    [string]$ProjectRoot = "",

    [int]$BuildTimeoutMinutes = 45
)

$ErrorActionPreference = "Stop"

function Get-ProjectRoot {
    if ($ProjectRoot -and (Test-Path $ProjectRoot)) {
        return (Resolve-Path $ProjectRoot).Path
    }
    # scripts/ -> gh-release/ -> skills/ -> .cursor/ -> repo
    $candidate = Resolve-Path (Join-Path $PSScriptRoot "..\..\..\..") -ErrorAction SilentlyContinue
    if ($candidate -and (Test-Path (Join-Path $candidate "ProjectSettings\ProjectSettings.asset"))) {
        return $candidate.Path
    }
    $cwd = (Get-Location).Path
    if (Test-Path (Join-Path $cwd "ProjectSettings\ProjectSettings.asset")) {
        return $cwd
    }
    throw "Cannot find Unity project root (ProjectSettings/ProjectSettings.asset)."
}

function Get-BundleVersion([string]$settingsPath) {
    $line = Select-String -Path $settingsPath -Pattern '^\s*bundleVersion:\s*(.+)$' | Select-Object -First 1
    if (-not $line) { throw "bundleVersion not found in $settingsPath" }
    return $line.Matches[0].Groups[1].Value.Trim()
}

function Set-BundleVersion([string]$settingsPath, [string]$newVersion) {
    $content = Get-Content -Path $settingsPath -Raw
    $updated = [regex]::Replace($content, '(?m)^(\s*bundleVersion:\s*).+$', "`${1}$newVersion")
    if ($updated -eq $content) { throw "Failed to update bundleVersion in $settingsPath" }
    Set-Content -Path $settingsPath -Value $updated -NoNewline
}

function Get-ProposedMinorBump([string]$current) {
    $parts = $current.Split('.')
    if ($parts.Length -lt 2) {
        throw "bundleVersion '$current' is not MAJOR.MINOR.PATCH (or MAJOR.MINOR)"
    }
    $major = [int]$parts[0]
    $minor = [int]$parts[1]
    $minor++
    return "$major.$minor.0"
}

function Find-UnityEditor([string]$root) {
    $versionFile = Join-Path $root "ProjectSettings\ProjectVersion.txt"
    $editorVersion = (Select-String -Path $versionFile -Pattern 'm_EditorVersion:\s*(\S+)' | Select-Object -First 1).Matches[0].Groups[1].Value
    $unityExe = Join-Path ${env:ProgramFiles} "Unity\Hub\Editor\$editorVersion\Editor\Unity.exe"
    if (-not (Test-Path $unityExe)) {
        throw "Unity Editor not found at $unityExe (from ProjectVersion.txt: $editorVersion)"
    }
    return $unityExe
}

function Test-UnityEditorOpen([string]$root) {
    $lockFile = Join-Path $root "Temp\UnityLockfile"
    return Test-Path $lockFile
}

function Invoke-UnityBatchBuild([string]$root) {
    $unity = Find-UnityEditor $root
    $logFile = Join-Path $root "Builds\Android\gh-release-build.log"
    $logDir = Split-Path $logFile -Parent
    if (-not (Test-Path $logDir)) { New-Item -ItemType Directory -Path $logDir | Out-Null }

    Write-Host "Editor closed — building APK with Unity batchmode..."
    Write-Host "Log: $logFile"
    $unityArgs = @(
        "-quit", "-batchmode", "-nographics",
        "-projectPath", $root,
        "-executeMethod", "Grodowladcy.Editor.CiBuild.BuildAndroidApk",
        "-logFile", $logFile
    )
    $proc = Start-Process -FilePath $unity -ArgumentList $unityArgs -Wait -PassThru -NoNewWindow
    if ($proc.ExitCode -ne 0) {
        throw "Unity batch build failed (exit $($proc.ExitCode)). See $logFile"
    }
}

function Invoke-UnityOpenEditorBuild([string]$root) {
    $tempDir = Join-Path $root "Temp"
    if (-not (Test-Path $tempDir)) { New-Item -ItemType Directory -Path $tempDir | Out-Null }

    $request = Join-Path $root "Temp\gh-release-build.request"
    $result = Join-Path $root "Temp\gh-release-build.result"

    if (Test-Path $result) { Remove-Item $result -Force }
    if (Test-Path $request) { Remove-Item $request -Force }

    Write-Host "Editor open (UnityLockfile) — requesting build in the running Editor..."
    Write-Host "Keep Unity focused/running; do not start a second Editor."
    # Tiny payload so the file is non-empty and easy to spot in Explorer
    Set-Content -Path $request -Value "build" -NoNewline

    $deadline = (Get-Date).AddMinutes($BuildTimeoutMinutes)
    while ((Get-Date) -lt $deadline) {
        if (Test-Path $result) {
            $code = (Get-Content $result -Raw).Trim()
            Remove-Item $result -Force -ErrorAction SilentlyContinue
            if ($code -eq "0") {
                Write-Host "Open-Editor build succeeded."
                return
            }
            throw "Open-Editor build failed (result=$code). Check Unity Console."
        }
        Start-Sleep -Seconds 2
    }

    throw "Timed out after $BuildTimeoutMinutes min waiting for open Editor build. Is CiBuild watcher compiled? Menu: Grodowladcy/Build Android APK (gh-release)."
}

$root = Get-ProjectRoot
$settings = Join-Path $root "ProjectSettings\ProjectSettings.asset"
$apkRel = "Builds\Android\Grodowladcy.apk"
$apkPath = Join-Path $root $apkRel

Write-Host "Project: $root"
$current = Get-BundleVersion $settings
$proposed = Get-ProposedMinorBump $current
Write-Host "Current Unity bundleVersion: $current"
Write-Host "Proposed (minor bump):       $proposed"

if ([string]::IsNullOrWhiteSpace($Version)) {
    $inputVersion = Read-Host "Version [$proposed] (Enter = accept, or type your own)"
    if ([string]::IsNullOrWhiteSpace($inputVersion)) {
        $Version = $proposed
    } else {
        $Version = $inputVersion.Trim().TrimStart('v', 'V')
    }
} else {
    $Version = $Version.Trim().TrimStart('v', 'V')
}

if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    throw "Version must look like X.Y.Z (got '$Version')"
}

if ([string]::IsNullOrWhiteSpace($Notes)) {
    throw "Release notes are required. Pass -Notes '...' or invoke /gh-release with notes as the argument."
}

Write-Host "Using version: $Version"
Set-BundleVersion $settings $Version
Write-Host "Updated bundleVersion → $Version"

if (-not $SkipBuild) {
    if (Test-UnityEditorOpen $root) {
        Invoke-UnityOpenEditorBuild $root
    } else {
        Invoke-UnityBatchBuild $root
    }
}

if (-not (Test-Path $apkPath)) {
    throw "APK not found at $apkPath"
}

$tag = "v$Version"
$title = "Grodowładcy v$Version"

Write-Host "Creating GitHub release $tag ..."
gh release create $tag $apkPath --title $title --notes $Notes

Write-Host "Done: $title"
gh release view $tag --json url -q .url
