

namespace Domain.Interfaces;

public interface IOrdering
{
    string Key { get; }

    SortOrder Order { get; }
}