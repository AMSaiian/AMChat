using System.Collections.ObjectModel;

namespace AMChat.Core.Interfaces;

public interface IOrdering
{
    public static abstract ReadOnlyDictionary<string, dynamic> OrderedBy { get; }
}
