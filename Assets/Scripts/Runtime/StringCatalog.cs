using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Grodowladcy
{
    /// <summary>
    /// Flat JSON map of player-facing strings. A missing key is returned as [key].
    /// </summary>
    public sealed class StringCatalog
    {
        private readonly Dictionary<string, string> entries;

        private StringCatalog(Dictionary<string, string> entries)
        {
            this.entries = entries;
        }

        public static StringCatalog Parse(string json)
        {
            if (json == null)
            {
                throw new ArgumentNullException(nameof(json));
            }

            return new StringCatalog(ParseEntries(json));
        }

        public string Get(string key)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("Key is required.", nameof(key));
            }

            if (entries.TryGetValue(key, out var value))
            {
                return value;
            }

            return "[" + key + "]";
        }

        private static Dictionary<string, string> ParseEntries(string json)
        {
            var parser = new Parser(json);
            parser.SkipWhitespace();
            parser.Expect('{');
            var entries = new Dictionary<string, string>();
            parser.SkipWhitespace();
            if (parser.TryConsume('}'))
            {
                parser.SkipWhitespace();
                parser.ExpectEnd();
                return entries;
            }

            while (true)
            {
                parser.SkipWhitespace();
                var key = parser.ReadString();
                if (key.Length == 0)
                {
                    throw new FormatException("String catalog key is empty.");
                }

                parser.SkipWhitespace();
                parser.Expect(':');
                parser.SkipWhitespace();
                var value = parser.ReadString();
                if (entries.ContainsKey(key))
                {
                    throw new FormatException("Duplicate string catalog key '" + key + "'.");
                }

                entries.Add(key, value);
                parser.SkipWhitespace();
                if (parser.TryConsume(','))
                {
                    continue;
                }

                parser.Expect('}');
                break;
            }

            parser.SkipWhitespace();
            parser.ExpectEnd();
            return entries;
        }

        private sealed class Parser
        {
            private readonly string json;
            private int index;

            public Parser(string json)
            {
                this.json = json;
                if (json.Length > 0 && json[0] == '\uFEFF')
                {
                    index = 1;
                }
            }

            public void SkipWhitespace()
            {
                while (index < json.Length && IsWhitespace(json[index]))
                {
                    index++;
                }
            }

            public bool TryConsume(char expected)
            {
                if (index < json.Length && json[index] == expected)
                {
                    index++;
                    return true;
                }

                return false;
            }

            public void Expect(char expected)
            {
                if (!TryConsume(expected))
                {
                    throw new FormatException("String catalog expected '" + expected + "'.");
                }
            }

            public void ExpectEnd()
            {
                if (index != json.Length)
                {
                    throw new FormatException("String catalog has trailing content.");
                }
            }

            public string ReadString()
            {
                Expect('"');
                var builder = new StringBuilder();
                while (index < json.Length)
                {
                    var current = json[index];
                    index++;
                    if (current == '"')
                    {
                        return builder.ToString();
                    }

                    if (current == '\\')
                    {
                        builder.Append(ReadEscape());
                        continue;
                    }

                    if (current < ' ')
                    {
                        throw new FormatException("String catalog string contains a control character.");
                    }

                    builder.Append(current);
                }

                throw new FormatException("String catalog string is unterminated.");
            }

            private char ReadEscape()
            {
                if (index >= json.Length)
                {
                    throw new FormatException("String catalog escape is unterminated.");
                }

                var current = json[index];
                index++;
                switch (current)
                {
                    case '"':
                    case '\\':
                    case '/':
                        return current;
                    case 'b':
                        return '\b';
                    case 'f':
                        return '\f';
                    case 'n':
                        return '\n';
                    case 'r':
                        return '\r';
                    case 't':
                        return '\t';
                    case 'u':
                        return ReadHexChar();
                    default:
                        throw new FormatException("String catalog has an unknown escape.");
                }
            }

            private char ReadHexChar()
            {
                if (index + 4 > json.Length)
                {
                    throw new FormatException("String catalog unicode escape is truncated.");
                }

                var hex = json.Substring(index, 4);
                index += 4;
                if (!ushort.TryParse(hex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var code))
                {
                    throw new FormatException("String catalog unicode escape is invalid.");
                }

                return (char)code;
            }

            private static bool IsWhitespace(char current)
            {
                return current == ' ' || current == '\t' || current == '\n' || current == '\r';
            }
        }
    }
}
