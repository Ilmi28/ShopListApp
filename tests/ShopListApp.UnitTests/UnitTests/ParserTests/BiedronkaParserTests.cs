using ShopListApp.Core.Commands.Other;
using ShopListApp.Infrastructure.Parsers;
using ShopListApp.TestUtilities.Comparers;
using ShopListApp.TestUtilities.Stubs;

namespace ShopListApp.UnitTests.UnitTests.ParserTests
{
    public class BiedronkaParserTests
    {
        private BiedronkaParser _parser;
        public BiedronkaParserTests()
        {
            var fetcher = new BiedronkaHtmlFetcherStub();
            _parser = new BiedronkaParser(fetcher);
        }

        [Fact]
        public async Task GetParsedProducts_ReturnsAllProducts()
        {
            ICollection<ParseProductCommand> products = await _parser.GetParsedProducts();

            Assert.Equal(ExpectedProducts(), products, new ParsedProductComparer());
        }

        private ICollection<ParseProductCommand> ExpectedProducts()
        {
            return new List<ParseProductCommand>
            {
                new ParseProductCommand
                {
                    Name = "Ziemniak myty jadalny 2 kg",
                    Price = 8.99m,
                    CategoryName = "warzywa",
                    ImageUrl = "https://zakupy.biedronka.pl/dw/image/v2/BKFJ_PRD/on/demandware.static/-/Sites-PL_Master_Catalog/default/dw71a3cfd1/images/hi-res/C810AA40311CB25D7C906E026BD7EE7C.jpg?sw=270&amp;sh=270&amp;sm=fit",
                    StoreId = 1
                },
                new ParseProductCommand
                {
                    Name = "Papryka Czerwona luz",
                    Price = 16.99m,
                    CategoryName = "warzywa",
                    ImageUrl = "https://zakupy.biedronka.pl/dw/image/v2/BKFJ_PRD/on/demandware.static/-/Sites-PL_Master_Catalog/default/dw001ca5a6/images/hi-res/125571a.jpg?sw=270&amp;sh=270&amp;sm=fit",
                    StoreId = 1
                },
                new ParseProductCommand
                {
                    Name = "Ogórki szklarniowe kg",
                    Price = 14.99m,
                    CategoryName = "warzywa",
                    ImageUrl = "https://zakupy.biedronka.pl/dw/image/v2/BKFJ_PRD/on/demandware.static/-/Sites-PL_Master_Catalog/default/dwef4e8c68/images/hi-res/125532.jpg?sw=270&amp;sh=270&amp;sm=fit",
                    StoreId = 1
                }
            };
        }
    }
}
