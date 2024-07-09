using System.Collections.ObjectModel;

namespace AMChat.Core.Interfaces;

public interface IOrdered
{
    public static abstract ReadOnlyDictionary<string, dynamic> OrderedBy { get; }
}
