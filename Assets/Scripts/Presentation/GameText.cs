using System;
using UnityEngine;

namespace Grodowladcy.Presentation
{
    /// <summary>
    /// Loads the Polish string catalog from Resources and returns player-facing text by key.
    /// </summary>
    public static class GameText
    {
        private const string ResourcePath = "Localization/pl";

        private static StringCatalog catalog;

        public static string Get(string key)
        {
            if (catalog == null)
            {
                var asset = Resources.Load<TextAsset>(ResourcePath);
                if (asset == null)
                {
                    throw new InvalidOperationException(
                        "Missing string catalog at Resources/" + ResourcePath + ".");
                }

                catalog = StringCatalog.Parse(asset.text);
            }

            return catalog.Get(key);
        }
    }
}
