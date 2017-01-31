using Lucene.Net.Support;
using System;
using System.Collections.Generic;

namespace Lucene.Net.Util
{
    /*
     * Licensed to the Apache Software Foundation (ASF) under one or more
     * contributor license agreements.  See the NOTICE file distributed with
     * this work for additional information regarding copyright ownership.
     * The ASF licenses this file to You under the Apache License, Version 2.0
     * (the "License"); you may not use this file except in compliance with
     * the License.  You may obtain a copy of the License at
     *
     *     http://www.apache.org/licenses/LICENSE-2.0
     *
     * Unless required by applicable law or agreed to in writing, software
     * distributed under the License is distributed on an "AS IS" BASIS,
     * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
     * See the License for the specific language governing permissions and
     * limitations under the License.
     */

    /// <summary>
    /// Methods for manipulating (sorting) collections.
    /// Sort methods work directly on the supplied lists and don't copy to/from arrays
    /// before/after. For medium size collections as used in the Lucene indexer that is
    /// much more efficient.
    ///
    /// @lucene.internal
    /// </summary>
    public sealed class CollectionUtil
    {
        private CollectionUtil() // no instance
        {
        }

        private sealed class ListIntroSorter<T> : IntroSorter
        {
            internal T pivot;
            internal IList<T> list;
            internal readonly IComparer<T> comp;

            internal ListIntroSorter(IList<T> list, IComparer<T> comp)
                : base()
            {
                /* LUCENE TO-DO I believe all ILists are RA
                if (!(list is RandomAccess))
                {
                  throw new System.ArgumentException("CollectionUtil can only sort random access lists in-place.");
                }*/
                this.list = list;
                this.comp = comp;
            }

            protected override void SetPivot(int i)
            {
                pivot = (i < list.Count) ? list[i] : default(T);
            }

            protected override void Swap(int i, int j)
            {
                list = list.Swap(i, j); // LUCENENET TODO: Could be more efficient
            }

            protected override int Compare(int i, int j)
            {
                return comp.Compare(list[i], list[j]);
            }

            protected override int ComparePivot(int j)
            {
                return comp.Compare(pivot, list[j]);
            }
        }

        private sealed class ListTimSorter<T> : TimSorter
        {
            internal IList<T> list;
            internal readonly IComparer<T> comp;
            internal readonly T[] tmp;

            internal ListTimSorter(IList<T> list, IComparer<T> comp, int maxTempSlots)
                : base(maxTempSlots)
            {
                /* LUCENE TO-DO I believe all ILists are RA
                if (!(list is RandomAccess))
                {
                  throw new System.ArgumentException("CollectionUtil can only sort random access lists in-place.");
                }*/
                this.list = list;
                this.comp = comp;
                if (maxTempSlots > 0)
                {
                    this.tmp = new T[maxTempSlots];
                }
                else
                {
                    this.tmp = null;
                }
            }

            protected override void Swap(int i, int j)
            {
                list = list.Swap(i, j); // LUCENENET TODO: Could be more efficient
            }

            protected override void Copy(int src, int dest)
            {
                list[dest] = list[src];
            }

            protected override void Save(int i, int len)
            {
                for (int j = 0; j < len; ++j)
                {
                    tmp[j] = list[i + j];
                }
            }

            protected override void Restore(int i, int j)
            {
                list[j] = tmp[i];
            }

            protected override int Compare(int i, int j)
            {
                return comp.Compare(list[i], list[j]);
            }

            protected override int CompareSaved(int i, int j)
            {
                return comp.Compare(tmp[i], list[j]);
            }
        }

        /// <summary>
        /// Sorts the given random access <seealso cref="List"/> using the <seealso cref="Comparer"/>.
        /// The list must implement <seealso cref="RandomAccess"/>. this method uses the intro sort
        /// algorithm, but falls back to insertion sort for small lists. </summary>
        /// <exception cref="IllegalArgumentException"> if list is e.g. a linked list without random access. </exception>
        public static void IntroSort<T>(IList<T> list, IComparer<T> comp)
        {
            int size = list.Count;
            if (size <= 1)
            {
                return;
            }
            (new ListIntroSorter<T>(list, comp)).Sort(0, size);
        }

        /// <summary>
        /// Sorts the given random access <seealso cref="List"/> in natural order.
        /// The list must implement <seealso cref="RandomAccess"/>. this method uses the intro sort
        /// algorithm, but falls back to insertion sort for small lists. </summary>
        /// <exception cref="IllegalArgumentException"> if list is e.g. a linked list without random access. </exception>
        public static void IntroSort<T>(IList<T> list)
            //where T : IComparable<T> // LUCENENET specific: removing constraint because in .NET, it is not needed
        {
            int size = list.Count;
            if (size <= 1)
            {
                return;
            }
            IntroSort(list, ArrayUtil.NaturalComparer<T>());
        }

        // Tim sorts:

        /// <summary>
        /// Sorts the given random access <seealso cref="List"/> using the <seealso cref="Comparer"/>.
        /// The list must implement <seealso cref="RandomAccess"/>. this method uses the Tim sort
        /// algorithm, but falls back to binary sort for small lists. </summary>
        /// <exception cref="IllegalArgumentException"> if list is e.g. a linked list without random access. </exception>
        public static void TimSort<T>(IList<T> list, IComparer<T> comp)
        {
            int size = list.Count;
            if (size <= 1)
            {
                return;
            }
            (new ListTimSorter<T>(list, comp, list.Count / 64)).Sort(0, size);
        }

        /// <summary>
        /// Sorts the given random access <seealso cref="List"/> in natural order.
        /// The list must implement <seealso cref="RandomAccess"/>. this method uses the Tim sort
        /// algorithm, but falls back to binary sort for small lists. </summary>
        /// <exception cref="IllegalArgumentException"> if list is e.g. a linked list without random access. </exception>
        public static void TimSort<T>(IList<T> list)
            //where T : IComparable<T> // LUCENENET specific: removing constraint because in .NET, it is not needed
        {
            int size = list.Count;
            if (size <= 1)
            {
                return;
            }
            TimSort(list, ArrayUtil.NaturalComparer<T>());
        }
    }
}