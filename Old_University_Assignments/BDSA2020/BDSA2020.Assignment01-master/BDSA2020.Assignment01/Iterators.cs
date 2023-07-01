using System;
using System.Collections.Generic;

namespace BDSA2019.Assignment01
{
    public static class Iterators
    {
        public static IEnumerable<T> Flatten<T>(IEnumerable<IEnumerable<T>> items)
        {
            foreach(IEnumerable<T> stream in items) {
                foreach(T item in stream) {
                    yield return item;
                }
            }
        }

        public static IEnumerable<T> Filter<T>(IEnumerable<T> items, Predicate<T> predicate)
        {
            foreach(T item in items) {
                if (predicate(item)) {
                    yield return item;
                }
            }
        }
    }
}
