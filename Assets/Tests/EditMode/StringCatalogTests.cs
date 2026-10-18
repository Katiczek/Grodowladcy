using System;
using System.IO;
using Grodowladcy;
using NUnit.Framework;
using UnityEngine;

namespace Grodowladcy.Tests.EditMode
{
    public class StringCatalogTests
    {
        [Test]
        public void Get_ReturnsValue_When_KeyExists()
        {
            var catalog = StringCatalog.Parse("{ \"menu.resume\": \"Wznow\" }");

            Assert.AreEqual("Wznow", catalog.Get("menu.resume"));
        }

        [Test]
        public void Get_ReturnsBracketedKey_When_KeyMissing()
        {
            var catalog = StringCatalog.Parse("{ \"menu.resume\": \"Wznow\" }");

            Assert.AreEqual("[menu.new_game]", catalog.Get("menu.new_game"));
        }

        [Test]
        public void Parse_Throws_When_KeyIsDuplicated()
        {
            Assert.Throws<FormatException>(() =>
                StringCatalog.Parse("{ \"a\": \"1\", \"a\": \"2\" }"));
        }

        [Test]
        public void Parse_Throws_When_JsonIsMalformed()
        {
            Assert.Throws<FormatException>(() => StringCatalog.Parse("{ \"a\": "));
        }

        [Test]
        public void Parse_ReadsEscapedQuote()
        {
            var catalog = StringCatalog.Parse("{ \"menu.quote\": \"powiedzial \\\"tak\\\"\" }");

            Assert.AreEqual("powiedzial \"tak\"", catalog.Get("menu.quote"));
        }

        [Test]
        public void PolishCatalog_ContainsMenuStrings()
        {
            var path = Path.Combine(Application.dataPath, "Resources", "Localization", "pl.json");
            var catalog = StringCatalog.Parse(File.ReadAllText(path));

            Assert.AreEqual("Nowa rozgrywka", catalog.Get("menu.new_game"));
            Assert.AreEqual("Wznów", catalog.Get("menu.resume"));
            Assert.AreEqual("Brak zapisu do wznowienia.", catalog.Get("menu.resume.no_save"));
        }
    }
}
