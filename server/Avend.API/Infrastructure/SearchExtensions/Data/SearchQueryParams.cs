using Avend.API.Validation.Util;
using Qoden.Validation;

namespace Avend.API.Infrastructure.SearchExtensions.Data
{
    /// <summary>
    /// Hold common search parameters such as pagination, sorting and filtering
    /// </summary>
    public class SearchQueryParams : ISortingQueryParams, IFilterQueryParams
    {
        public static readonly string[] AllowedSortOrderValues = { "asc", "desc" };

        public SearchQueryParams(string filter = null, string sortField = null, string sortOrder = "asc",
            int pageNumber = 0, int recordsPerPage = 25)
        {
            Filter = filter;
            SortField = sortField;
            SortOrder = sortOrder;
            PageNumber = pageNumber;
            RecordsPerPage = recordsPerPage;
        }

        public string Filter { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
        public int PageNumber { get; set; }
        public int RecordsPerPage { get; set; }

        public IValidator Validate(IValidator validator = null)
        {
            validator = validator ?? new Validator();
            if (Filter != null)
            {
                validator.CheckValue(Filter, "q").MaxLength(256);
            }
            if (SortField != null)
            {
                validator.CheckValue(SortOrder, "sort_order").In(new[] { "asc", "desc" });
            }
            validator.CheckValue(PageNumber, "page_number").GreaterOrEqualTo(0);
            validator.CheckValue(RecordsPerPage, "per_page").BetweenInclusive(1, 100);
            return validator;
        }
    }
}