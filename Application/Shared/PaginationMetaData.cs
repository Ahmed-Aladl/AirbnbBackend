namespace Application.Shared
{
    /// <summary>
    /// Represents Pagination Meta data will returned from Repositories.
    /// </summary>
    public class PaginationMetaData
    {
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int Total { get; set; }
    }
}
