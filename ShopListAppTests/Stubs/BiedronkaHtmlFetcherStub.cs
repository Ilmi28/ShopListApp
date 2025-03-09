using HtmlAgilityPack;
using ShopListApp.DataProviders;
using ShopListApp.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopListAppTests.Stubs
{
    class BiedronkaHtmlFetcherStub : HAPHtmlFetcher
    {
        public override async Task<string?> FetchHtml(string url, string? uri = null)
        {
            if (url == "https://zakupy.biedronka.pl/" && uri!.Contains("warzywa"))
            {
                string filePath = "TestData/BiedronkaPage.html";
                using var reader = new StreamReader(filePath, true);
                string html = await reader.ReadToEndAsync();
                return html;
            }
            return null;
        }
    }
}
