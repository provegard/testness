using System.Collections.Generic;

namespace GraphBuilder.Test
{
    public interface IHasChildren<out T>
    {
        IEnumerable<T> GetChildren();
    }
}
