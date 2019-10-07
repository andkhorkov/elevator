using System;
using System.Collections.Generic;

public class PriorityUQueue<T> where T : IComparable<T>
{
    private List<T> data;
    private HashSet<T> set;

    public int Count => data.Count;

    public T Peek => data[0];

    public PriorityUQueue()
    {
        data = new List<T>();
        set = new HashSet<T>();
    }

    public bool Contains(T item)
    {
        return set.Contains(item);
    }

    public void Remove(T item)
    {
        data.Remove(item);
        Heapify(0, data.Count - 1);
    }

    public void Enqueue(T item)
    {
        if (set.Contains(item))
        {
            return;
        }

        data.Add(item);
        set.Add(item);
        var child = data.Count - 1;

        while (child > 0)
        {
            var parent = (child - 1) / 2;

            if (data[child].CompareTo(data[parent]) >= 0)
            {
                break;
            }

            Swap(child, parent, data);
            child = parent;
        }
    }

    private static void Swap(int a, int b, IList<T> val)
    {
        var temp = val[a];
        val[a] = val[b];
        val[b] = temp;
    }

    private void Heapify(int parent, int last)
    {
        while (true)
        {
            int lChild = parent * 2 + 1;

            if (lChild > last)
            {
                break;
            }

            var rChild = lChild + 1;

            if (rChild <= last && data[rChild].CompareTo(data[lChild]) < 0)
            {
                lChild = rChild;
            }

            if (data[parent].CompareTo(data[lChild]) <= 0)
            {
                break;
            }

            Swap(parent, lChild, data);
            parent = lChild;
        }
    }

    public T Dequeue()
    {
        var last = data.Count - 1; 
        var firstElement = data[0];   
        data[0] = data[last];
        data.RemoveAt(last);

        --last; 
        var parent = 0; 

        Heapify(parent, last);

        set.Remove(firstElement);

        return firstElement;
    }
} 
