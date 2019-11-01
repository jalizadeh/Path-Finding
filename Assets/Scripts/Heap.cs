using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T>
{
    T[] items;
    int currentItemCount;

    //Initialize the heap with the maximum size it needs
    public Heap(int maxHeapSize)
    {
        items = new T[maxHeapSize];
    }


    //Add a new item to the heap
    // add at the end of the heap, then sort the heap
    public void Add(T item)
    {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        currentItemCount++;
        SortUp(item);
    }


    //Sort the new item to the root, while it is bigger than it`s parent
    // in this project, bigger means `Lower F cost`, so the lower is fCost, the higher
    // is the item's priority
    
    //NOTE: in implementing the `Compare` method, the result are negated to cover
    // the rule above
    private void SortUp(T item)
    {
        int parentIndex = (item.HeapIndex - 1) / 2;

        while (true)
        {
            T parent = items[parentIndex];
            if(item.CompareTo(parent) > 0) //higher priorit = lower F cost
            {
                Swap(item, parent);

            } else
            {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }
    }

    //Swapping two items in the array
    private void Swap(T itemA, T itemB)
    {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;

        int tmpItemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = tmpItemAIndex;
    }


    //Remove the first iteam from the heap's tip
    // 1. Remove the first item
    // 2. Put the last item, into the first item
    // 3. Sort it down-ward (compare to it's possible children)
    public T RemoveFirst() {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);

        return firstItem;
    }


    //Sort the new inserted item (at the top), by:
    // 1. Check if it has any children
    // 2. Compare which child has the lowest F cost
    // 3. Compare if the item and child's F cost, then swap if OK
    private void SortDown(T item)
    {
        while (true)
        {
            int childIndexLeft = (item.HeapIndex * 2) + 1;
            int childIndexRight = (item.HeapIndex * 2) + 2;
            int swapIndex = 0;


            if (childIndexLeft < currentItemCount)
            {
                swapIndex = childIndexLeft;

                if (childIndexRight < currentItemCount)
                {
                    if (items[childIndexLeft].CompareTo(items[childIndexRight]) < 0)
                    {
                        swapIndex = childIndexRight;
                    }
                }

                if(item.CompareTo(items[swapIndex]) < 0)
                {
                    Swap(item, items[swapIndex]);
                } else
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

    //Check if the item exists in the heap
    public bool Contains(T item)
    {
        return Equals(items[item.HeapIndex], item);
    }


    //Return the current size of the heap
    public int Count
    {
        get {
            return currentItemCount;
        }
    }


    //Update the item and based on this change, update the heap (if needed)
    //Note: For this project, the lower F are kept in the begining (the heap is in ASCending order)
    // so, we just need to check if the updated item, is lower than the parent or not
    // If the values were in DECending order, we had to use `SortDown`
    public void UpdateItem(T item)
    {
        SortUp(item);
    }


}


public interface IHeapItem<T> : IComparable<T> {
    int HeapIndex {
        get;
        set;
    }
}
