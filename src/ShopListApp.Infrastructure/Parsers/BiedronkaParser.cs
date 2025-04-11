using HtmlAgilityPack;
using ShopListApp.Core.Commands.Other;
using ShopListApp.Core.Interfaces.Parsing;
using System.Text;

namespace ShopListApp.Infrastructure.Parsers;

public class BiedronkaParser(IHtmlFetcher<HtmlNode, HtmlDocument> htmlFetcher) : IParser
{
    private IDictionary<string, string> CategoryOnPageNameToCategoryInDb()
    {
        var dict = new Dictionary<string, string>
        {
            { "warzywa", "warzywa" },
            { "owoce", "owoce" },
            { "piekarnia", "pieczywa" },
            { "nabial", "nabial i jajka" },
            { "mieso", "mieso" },
            { "dania-gotowe", "dania gotowe" },
            { "napoje", "napoje" },
            { "mrozone", "mrozone" },
            { "artykuly-spozywcze", "artykuly spozywcze" },
            { "drogeria", "drogeria" },
            { "dla-domu", "dla domu" },
            { "dla-dzieci", "dla dzieci" },
            { "dla-zwierzat", "dla zwierzat" },
        };
        return dict;
    }

    public async Task<ICollection<ParseProductCommand>> GetParsedProducts()
    {
        string baseUri = "https://zakupy.biedronka.pl/";
        var categories = CategoryOnPageNameToCategoryInDb();
        var allProducts = new List<ParseProductCommand>();
        foreach (var category in categories)
        {
            var html = new HtmlDocument();
            var fetched = await htmlFetcher.FetchHtml(baseUri, category.Key);
            if (fetched == null) continue;
            html.LoadHtml(fetched);
            var pages = htmlFetcher.GetElementsByClassName(html, "bucket-pagination__link");
            int amountOfPages = int.Parse(pages.Last().InnerHtml);
            for (int i = 1; i <= amountOfPages; i++)
            {
                html = new HtmlDocument();
                string uri = $"{category.Key}?page={i}";
                fetched = await htmlFetcher.FetchHtml(baseUri, uri);
                html.LoadHtml(fetched);
                var pageProducts = FetchProductsFromPage(html, category.Value);
                allProducts.AddRange(pageProducts);
            }
        }
        return allProducts;
    }

    private ICollection<ParseProductCommand> FetchProductsFromPage(HtmlDocument html, string? dbCategory)
    {
        var products = new List<ParseProductCommand>();
        var productsHtml = htmlFetcher.GetElementsByClassName(html, "js-product-tile");
        foreach (var productHtml in productsHtml)
        {
            var productTileHtml = new HtmlDocument();
            productTileHtml.LoadHtml(productHtml.InnerHtml);
            var imageContainer = htmlFetcher.GetElementsByClassName(productTileHtml, "tile-image__container").FirstOrDefault();
            var imgNode = imageContainer!.SelectSingleNode("//img");
            var product = new ParseProductCommand
            {
                Name = htmlFetcher.GetElementsByClassName(productTileHtml, "product-tile__name").First().InnerHtml.Trim(),
                Price = ParsePrice(productTileHtml),
                ImageUrl = htmlFetcher.GetAttributeValue(imgNode!, "data-srcset"),
                CategoryName = dbCategory ?? null,
                StoreId = 1
            };
            products.Add(product);
        }
        return products;
    }

    private decimal? ParsePrice(HtmlDocument htmlDoc)
    {
        string? intHtml = htmlFetcher.GetElementsByClassName(htmlDoc, "price-tile__sales").FirstOrDefault()!.InnerHtml;
        if (string.IsNullOrWhiteSpace(intHtml)) return null;
        var sb = new StringBuilder();
        foreach (char chr in intHtml)
        {
            if (chr == '<') break;
            sb.Append(chr);
        }
        ;
        string integerPart = sb.ToString().Trim();
        var decNode = htmlFetcher.GetElementsByClassName(htmlDoc, "price-tile__decimal").FirstOrDefault();
        string decimalPart = htmlFetcher.GetElementsByClassName(htmlDoc, "price-tile__decimal").FirstOrDefault()!.InnerHtml ?? "00";
        string fullNum = $"{integerPart},{decimalPart}";
        bool result = decimal.TryParse(fullNum, out decimal price);
        if (!result) return null;
        return price;
    }
}
