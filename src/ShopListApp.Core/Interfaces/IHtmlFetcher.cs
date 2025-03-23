

namespace ShopListApp.Interfaces
{
    // T - type of HTML node
    // Y - type of HTML document
    public interface IHtmlFetcher<T, Y>
    {
        Task<string?> FetchHtml(string url, string? uri = default);
        ICollection<T> GetElementsByClassName(Y htmlDoc, string className);
        T? GetElementById(Y htmlDoc, string id);
        string? GetAttributeValue(T htmlNode, string attributeName);
    }
}
