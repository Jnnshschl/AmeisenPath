using AmeisenPathCore.Interfaces;

namespace AmeisenPathCore.Objects
{
    /// <summary>
    /// Heap datastructure
    /// </summary>
    /// <typeparam name="T">type of data to store in the heap</typeparam>
    public class Heap<T> where T : IHeapItem<T>
    {
        public int Count { get; private set; }
        private T[] Items { get; set; }

        /// <summary>
        /// Data structure that is based on the BinaryHeap
        /// </summary>
        /// <param name="maxHeapSize"></param>
        public Heap(int maxHeapSize)
        {
            Items = new T[maxHeapSize];
        }

        /// <summary>
        /// Add an Item to the Heap
        /// </summary>
        /// <param name="item">item to insert into the heap</param>
        public void Add(T item)
        {
            item.HeapIndex = Count;
            Items[Count] = item;
            SortUp(item);
            Count++;
        }

        /// <summary>
        /// Remove an Item from the Heap
        /// </summary>
        /// <returns>item to remove from the heap</returns>
        public T RemoveFirst()
        {
            T firstItem = Items[0];
            Count--;
            Items[0] = Items[Count];
            Items[0].HeapIndex = 0;
            SortDown(Items[0]);
            return firstItem;
        }

        /// <summary>
        /// Update an item in the heap
        /// </summary>
        /// <param name="item">item to update</param>
        public void UpdateItem(T item)
        {
            SortUp(item);
        }

        /// <summary>
        /// Check if the Heap contains a specific Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public bool Contains(T item)
        {
            return Equals(Items[item.HeapIndex], item);
        }

        /// <summary>
        /// Sort the heap Downwards
        /// </summary>
        /// <param name="item">item that got popped out of the heap</param>
        private void SortDown(T item)
        {
            bool stopSorting = false;
            while (!stopSorting)
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
                        stopSorting = true;
                    }
                }
                else
                {
                    stopSorting = true;
                }
            }
        }

        /// <summary>
        /// Sort the heap Upwards
        /// </summary>
        /// <param name="item">item that got inserted into the heap</param>
        private void SortUp(T item)
        {
            bool stopSorting = false;
            int parentIndex = (item.HeapIndex - 1) / 2;

            while (!stopSorting)
            {
                T parentItem = Items[parentIndex];
                if (item.CompareTo(parentItem) > 0)
                {
                    Swap(item, parentItem);
                }
                else
                {
                    stopSorting = true;
                }

                parentIndex = (item.HeapIndex - 1) / 2;
            }
        }

        /// <summary>
        /// Swap to items inside the Heap
        /// </summary>
        /// <param name="a">item a</param>
        /// <param name="b">item b</param>
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