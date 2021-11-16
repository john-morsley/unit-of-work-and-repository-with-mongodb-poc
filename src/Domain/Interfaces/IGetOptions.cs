namespace Domain.Interfaces;

public interface IGetOptions
{
    int PageSize { get; }

    int PageNumber { get; }

    string? SearchQuery { get; }

    IEnumerable<IFilter> Filters { get; }

    IEnumerable<IOrdering> Orderings { get; }
}
