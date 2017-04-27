using System;
using System.Linq;

namespace Vk_Parser_Console.Classes
{
    public class Randomizer
    {
        private int[] _shuffled;
        private int _index;
        private readonly int _start;

        public int Count { get; }

        public Randomizer(int start, int end)
        {
            Count = end - start;
            var r = new Random();
            _shuffled = Enumerable.Range(start, Count).OrderBy(x => r.Next()).ToArray();
            _start = start;
        }

        public int Next()
        {
            if (_index == _shuffled.Length)
            {
                _index = 0;
            }
            return _shuffled[_index++];
        }

        public void Shuffle()
        {
            var r = new Random();
            _shuffled = Enumerable.Range(_start, Count).OrderBy(x => r.Next()).ToArray();
        }

        // Set random dates into array
        public static int [] SetRandomDateNums(int yearFrom, int yearTo)
        {
            var qtyYear = yearTo - yearFrom + 1;
            var yearsArray = (qtyYear & 1) == 1 ? new int [(qtyYear - 1) / 2] : new int[qtyYear / 2];

            if ((qtyYear & 1) == 1)
            {
                for (var i = 0; i < yearsArray.Length; i++) yearsArray[i] = 2;
                yearsArray[yearsArray.Length-1] = 3;
            }
            else
            {
                for (var i = 0; i < yearsArray.Length; i++) yearsArray[i] = 2;
            }

            var maxRandom = (yearTo - yearFrom + 1) * 365;
            var datesNumArray = new int[maxRandom];
            var iRandom = 0;

            for (var j = 0; j < yearsArray.Length; j++)
            {
                var jRandom = maxRandom - yearsArray[j] * 365;
                var ran = new Randomizer(jRandom, maxRandom);

                for (var i = iRandom; i < yearsArray[j]*365 + iRandom; i++) datesNumArray[i] = ran.Next();

                iRandom = iRandom + yearsArray[j]*365;
                maxRandom = jRandom;
            }
            return datesNumArray;
        }

        // Remove value from array
        public static int[] RemoveValue (int[] array, int item)
        {
            var remInd = -1;

            for (var i = 0; i < array.Length; ++i)
            {
                if (array[i] == item)
                {
                    remInd = i;
                    break;
                }
            }

            var retVal = new int [array.Length - 1];

            for (int i = 0, j = 0; i < retVal.Length; ++i, ++j)
            {
                if (j == remInd) ++j;
                retVal[i] = array[j];
            }
            return retVal;
        }
    }
}