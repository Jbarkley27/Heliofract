using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PixelNarval.HPBars
{
    public class PriorityEvent<T>
    {
        public  SortedDictionary<int, Action<T>> dic = new SortedDictionary<int, Action<T>>();

        public void Subscribe(Action<T> action, int order = 0)
        {
            if (!dic.ContainsKey(order))
            {
                dic.Add(order, action);
            }
            else
            {
                dic[order] += action;
            }
        }

        public void Unsubscribe(Action<T> action, int order = 0)
        {
            if (dic.ContainsKey(order))
            {
                dic[order] -= action;
                if (dic[order] == null)
                {
                    dic.Remove(order);
                }
            }
        }

        public void Invoke(T value)
        {
            foreach (var dicItems in dic.Values)
            {
                dicItems?.Invoke(value);
            }
        }
    }

}