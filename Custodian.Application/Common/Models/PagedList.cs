namespace Custodian.Application.Common.Models;

public record PagedList<T>(
    IReadOnlyList<T> Items,
    int PageNumber,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasNextPage,
    bool HasPreviousPage
)
{
    //---- Helper Factory Method ----
    public static PagedList<T> Create(IReadOnlyList<T> items, int totalCount, int pageNumber, int pageSize)
    {
        int totalPages = (int)Math.Ceiling(totalCount / (double)(pageSize > 0 ? pageSize : 10));
        bool hasNextPage = pageNumber < totalPages;
        bool hasPreviousPage = pageNumber > 1;

        return new PagedList<T>(items, pageNumber, pageSize, totalCount, totalPages, hasNextPage, hasPreviousPage);
    }
}
