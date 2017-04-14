namespace Avend.API.Infrastructure.SearchExtensions.Data
{
    /// <summary>
    /// Public interface to isolate sorting logic.
    /// </summary>
    public interface ISortingQueryParams
    {
        string SortField { get; }
        string SortOrder { get; }
    }
}