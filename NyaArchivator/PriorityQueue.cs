using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HuffmanNM
{
    internal class PriorityQueue<T>
    {
        int size;
        SortedDictionary<int, Queue<T>> dictionary;
        public PriorityQueue()
        {
            dictionary = new SortedDictionary<int, Queue<T>>();
            size = 0;
        }
        public int Size() => size;
        public void Enqueue(int priority, T item) // метод для добавления в очередь Enqueue  
        {
            if (!dictionary.ContainsKey(priority)) { 
                dictionary.Add(priority, new Queue<T>());   
            }
            dictionary[priority].Enqueue(item);
            size++;
        }
        public T Dequeue()
        {
            if (size == 0) { throw new System.Exception("Queue is empty"); }
            size--;
            foreach (Queue<T> q in dictionary.Values)
            {
                if (q.Count > 0) { return q.Dequeue(); }
            }
            throw new System.Exception("Queue error");
        }
    }
}
