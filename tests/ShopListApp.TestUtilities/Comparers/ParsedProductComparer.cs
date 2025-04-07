using ShopListApp.Core.Commands.Other;

namespace ShopListApp.TestUtilities.Comparers
{
    public class ParsedProductComparer : IEqualityComparer<ParseProductCommand>
    {
        public bool Equals(ParseProductCommand? x, ParseProductCommand? y)
        {
            if (x == null || y == null) return false;
            return x.Name == y.Name &&
                   x.Price == y.Price &&
                   x.CategoryName == y.CategoryName &&
                   x.ImageUrl == y.ImageUrl &&
                   x.StoreId == y.StoreId;
        }

        public int GetHashCode(ParseProductCommand obj)
        {
            return HashCode.Combine(obj.Name, obj.Price, obj.CategoryName, obj.ImageUrl, obj.StoreId);
        }
    }
}
