using System;

namespace AmeisenPathLib.interfaces
{
    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}