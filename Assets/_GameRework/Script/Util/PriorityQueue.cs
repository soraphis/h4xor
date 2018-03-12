using System;
using System.Collections.Generic;
using System.Linq;

/**
 * Priority Queue implementation from: http://programanddesign.com/cs/a-simple-priority-queue-in-cs/
 */

namespace _Game.ScriptRework.Util {
    public class PriorityQueue<TValue> : PriorityQueue<TValue, int> { }

    public class PriorityQueue<TValue, TPriority> where TPriority : IComparable
    {
        private SortedDictionary<TPriority, Queue<TValue>> dict = new SortedDictionary<TPriority, Queue<TValue>>();

        public int Count { get; private set; }
        public bool Empty { get { return Count == 0; } }

        
        /// <summary>
        /// returns true, when 'val' exists with priority 'pri'
        /// </summary>
        public bool Contains(TValue val, TPriority pri) {
            if (!dict.ContainsKey(pri)) return false;
            return dict[pri].Contains(val);
        }
        
        public void Enqueue(TValue val)
        {
            Enqueue(val, default(TPriority));
        }

        public void Enqueue(TValue val, TPriority pri)
        {
            ++Count;
            if (!dict.ContainsKey(pri)) dict[pri] = new Queue<TValue>();
            dict[pri].Enqueue(val);
        }

        public TValue Dequeue()
        {
            --Count;
            var item = dict.First();
            if (item.Value.Count == 1) dict.Remove(item.Key);
            return item.Value.Dequeue();
        }
    }
}

