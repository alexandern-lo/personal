namespace Avend.API.Infrastructure.SearchExtensions.Data
{
    /// <summary>
    /// Public interface to isolate filtering logic.
    /// </summary>
    public interface IFilterQueryParams
    {
        string Filter { get; }
    }
}