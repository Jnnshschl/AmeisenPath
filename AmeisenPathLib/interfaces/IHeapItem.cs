using System;

namespace AmeisenPathCore.Interfaces
{
    public interface IHeapItem<T> : IComparable<T>
    {
        int HeapIndex { get; set; }
    }
}