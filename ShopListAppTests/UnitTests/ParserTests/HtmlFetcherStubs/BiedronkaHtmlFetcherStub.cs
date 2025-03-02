using HtmlAgilityPack;
using ShopListApp.DataProviders;
using ShopListApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopListAppTests.UnitTests.ParserTests.HtmlFetcherStubs
{
    class BiedronkaHtmlFetcherStub : HAPHtmlFetcher
    {
        public override async Task<string?> FetchHtml(string url, string? uri = null)
        {
            if (url == "https://zakupy.biedronka.pl/" && uri == "warzywa")
            {
                string filePath = "UnitTests/ParserTests/HtmlFetcherStubs/HtmlTestData/BiedronkaPage.html";
                using var reader = new StreamReader(filePath, true);
                string html = await reader.ReadToEndAsync();
                return html;
            }
            return null;
        }
    }
}
