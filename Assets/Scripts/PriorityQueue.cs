using System;
using System.Collections.Generic;

public class PriorityQueue<T> where T : IComparable<T>
{
    private List<T> data;

    public int Count => data.Count;

    public T Peek => data[0];

    public PriorityQueue()
    {
        data = new List<T>();
    }

    public void Enqueue(T item)
    {
        data.Add(item);
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

    private void Swap(int a, int b, IList<T> data)
    {
        var temp = data[a];
        data[a] = data[b];
        data[b] = temp;
    }

    public T Dequeue()
    {
        var last = data.Count - 1; 
        var firstElement = data[0];   
        data[0] = data[last];
        data.RemoveAt(last);

        --last; 
        var parent = 0; 

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

        return firstElement;
    }
} 
