using Qoden.Validation;

namespace Avend.API.Infrastructure.SearchExtensions.Data
{
    /// <summary>
    /// Special container holding only sorting parameters
    /// </summary>
    public class SortingQueryParams : ISortingQueryParams
    {
        public SortingQueryParams(string sortField = null, string sortOrder = "asc")
        {
            SortField = sortField;
            SortOrder = sortOrder;
        }

        public string SortField { get; set; }
        public string SortOrder { get; set; }

        public IValidator Validate(IValidator errors = null)
        {
            errors = errors ?? new Validator();
            errors.CheckValue(SortOrder, "sort_order").In(SearchQueryParams.AllowedSortOrderValues);
            return errors;
        }
    }
}