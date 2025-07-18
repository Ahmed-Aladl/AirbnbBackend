namespace Application.Shared
{
    /// <summary>
    /// Represents Paginated Result will be returned from Repositories
    /// </summary>
    /// <typeparam name="TEntity">Entity Type</typeparam>
    public class PaginatedResult<TEntity>
    {
        public IEnumerable<TEntity> Items { get; set; }
        public PaginationMetaData MetaData { get; set; }
    }
}
