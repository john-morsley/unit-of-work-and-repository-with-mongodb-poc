namespace Domain.Interfaces;

public interface IPagedList<T> : IList<T>
{
    int CurrentPage { get; }

    int TotalPages { get; }

    int PageSize { get; }

    int TotalCount { get; }

    bool HasPrevious { get; }

    bool HasNext { get; }
}