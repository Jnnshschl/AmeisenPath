using AmeisenPathLib.interfaces;

namespace AmeisenPathLib.objects
{
    public class Heap<T> where T : IHeapItem<T>
    {
        public int Count { get; private set; }
        private T[] Items { get; set; }

        public Heap(int maxHeapSize)
        {
            Items = new T[maxHeapSize];
        }

        public void Add(T item)
        {
            item.HeapIndex = Count;
            Items[Count] = item;
            SortUp(item);
            Count++;
        }

        public T RemoveFirst()
        {
            T firstItem = Items[0];
            Count--;
            Items[0] = Items[Count];
            Items[0].HeapIndex = 0;
            SortDown(Items[0]);
            return firstItem;
        }

        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        public bool Contains(T item)
        {
            return Equals(Items[item.HeapIndex], item);
        }

        private void SortDown(T item)
        {
            while (true)
            {
                int childL = item.HeapIndex * 2 + 1;
                int childR = item.HeapIndex * 2 + 2;
                int swapIndex = 0;

                if (childL < Count)
                {
                    swapIndex = childL;

                    if (childR < Count)
                    {
                        if (Items[childL].CompareTo(Items[childR]) < 0)
                        {
                            swapIndex = childR;
                        }
                    }

                    if (item.CompareTo(Items[swapIndex]) < 0)
                    {
                        Swap(item, Items[swapIndex]);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void SortUp(T item)
        {
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (true)
            {
                T parentItem = Items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    break;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        private void Swap(T a, T b)
        {
            Items[a.HeapIndex] = b;
            Items[b.HeapIndex] = a;
            int itemAIndex = a.HeapIndex;
            a.HeapIndex = b.HeapIndex;
            b.HeapIndex = itemAIndex;
        }
    }
}