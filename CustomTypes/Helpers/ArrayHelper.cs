using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CustomTypes.Helpers
{
    public static class ArrayHelper
    {
        public static bool EqualByValue<T>(this T[] srcArray, T[] compare) where T : IEquatable<T>
        {
            int srcLen = srcArray.Length;
            int cmpLen = compare.Length;

            if (srcLen != cmpLen) { return false; }

            // create our container we can modify at will
            List<T> bucket = new();
            foreach (T item in srcArray) { bucket.Add(item); }

            // now remove items one at a time from the bucket...
            foreach (T item in compare)
            {
                // try removing it.  If it's not there to remove, it's not equal
                if (!bucket.Remove(item)) { return false; }
            }

            return bucket.Count == 0; // can return true, but might as well be sure we removed everything
        }

        public static bool EqualByValueAndOrder<T>(this T[] srcArray, T[] compare) where T : IEquatable<T>
        {
            int srcLen = srcArray.Length;
            int cmpLen = compare.Length;

            if (srcLen != cmpLen) { return false; }

            for (int i = 0; i < srcLen; i++)
            {
                if (!srcArray[i].Equals(compare[i])) { return false; }
            }

            return true;
        }


        public static bool ContainsArrayByValue<T>(this T[] srcArray, T[] find) where T : IEquatable<T>
        {
            foreach (T item in find)
            {
                if (!srcArray.Contains(item)) { return false; }
            }
            return true;
        }



        public static T[] Sort<T>(this T[] srcArray) where T : IComparable<T>
        {
            int srcLen = srcArray.Length;
            var result = new T[srcLen];
            List<int> Remaining = new();
            for (int i = 0; i < srcLen; i++) { Remaining.Add(i); }

            for (int resi = 0; resi < srcLen; resi++)
            {
                T worker = srcArray[Remaining[0]]; // grab the first item in the remaining items
                int nextIndexToRemove = -1;

                foreach (int rem in Remaining)
                {
                    if (worker.CompareTo(srcArray[rem]) >= 0)
                    {
                        worker = srcArray[rem];
                        nextIndexToRemove = rem;
                    }
                }

                result[resi] = worker;
                Remaining.Remove(nextIndexToRemove);
            }

            return result;
        }

        /// <summary>
        /// Gives you the items that are unique by overall equality.  For 
        /// finer control, see the LINQ Distinct method.
        /// </summary>
        public static T[] Unique<T>(this T[] srcArray) where T : IEquatable<T>
        {
            List<T> uniqueList = new List<T>();
            foreach (T t in srcArray)
            {
                if (uniqueList.Contains(t)) { continue; }
                uniqueList.Add(t);
            }

            return uniqueList.ToArray();
        }


        public static T[] Randomize<T>(this T[] srcArray, bool? useSlowRandom = null)
        {
            bool slow = useSlowRandom ?? false;

            List<T> randList = new List<T>();
            Random rng = new Random();
            int sleep = 2;

            foreach (T t in srcArray) 
            { 
                int ii = rng.Next(0, randList.Count);
                randList.Insert(ii, t);

                if (slow) { Thread.Sleep(sleep); sleep = (sleep + ii) % 10 + 1; }
            }

            return randList.ToArray();
        }



        public static string ToString(this char[] charArray)
        {
            StringBuilder sb = new StringBuilder();

            foreach (char c in charArray) { sb.Append(c); }

            return sb.ToString();
        }

        public static int[] FromNumber(long srcVal)
        {
            if (srcVal == 0) { return [ 0 ]; }

            List<int> digits = new();

            long worker = srcVal;

            while (worker != 0)
            {
                int digit = (int)(worker % 10);
                digits.Insert(0, digit);
                worker = (worker - digit) / 10;
            }

            return digits.ToArray();
        }




    }
}
