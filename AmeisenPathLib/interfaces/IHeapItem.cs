using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AmeisenPathLib.interfaces
{
    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}
