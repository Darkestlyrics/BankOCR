using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OCRReader.Helpers
{
    internal static class CharArrayHelper
    {
        /// <summary>
        ///   Converts an 2D array of <see cref="char" /> to a string
        /// </summary>
        /// <param name="array">The array to convert</param>
        /// <returns><see cref="string" />representation of the array</returns>
        internal static string ArrToString(char[][] array)
        {
            if (array.Length == 0) throw new ArgumentException("Value cannot be an empty collection.", nameof(array));
            var res = new StringBuilder();
            for (var i = 0; i < array.Length; i++)
            for (var j = 0; j < array[i].Length; j++)
                res.Append(array[i][j]);

            return res.ToString();
        }

        /// <summary>
        ///   Returns the characters at a certain index
        /// </summary>
        /// <param name="raw">The raw array of characters</param>
        /// <param name="position">The position of the character</param>
        /// <returns>
        ///   <see>
        ///     <cref>char[][]</cref>
        ///   </see>
        ///   characters at index
        /// </returns>
        internal static char[][] GetCharAtIndex(char[][] raw, int position)
        {
            var temp = new char[4][];
            Array.Clear(temp, 0, 4); //clear temp buffer
            for (var j = 0; j < raw.Length; j++)
                temp[j] = position == 0 ? raw[j].Take(3).ToArray() : raw[j].Skip(3 * position).Take(3).ToArray();

            return temp;
        }

        /// <summary>
        ///   Pads empty lines in an array with empty characters
        /// </summary>
        /// <param name="length">Length to Pad to</param>
        internal static char[] PadArray(int length)
        {
            return Enumerable.Repeat(' ', length).ToArray();
        }

        /// <summary>
        ///   Gets all Indexes of a substring within a string
        /// </summary>
        /// <param name="str">The String to Search</param>
        /// <param name="searchstring">The string to search for</param>
        /// <returns>
        ///   <see>
        ///     <cref>IEnumerable{int}</cref>
        ///   </see>
        ///   of all Indexes
        /// </returns>
        internal static IEnumerable<int> AllIndexesOf(string str, string searchstring)
        {
            if (str == null) throw new ArgumentNullException(nameof(str));
            if (searchstring == null) throw new ArgumentNullException(nameof(searchstring));
            var minIndex = str.IndexOf(searchstring, StringComparison.Ordinal);
            while (minIndex != -1)
            {
                yield return minIndex;
                minIndex = str.IndexOf(searchstring, minIndex + searchstring.Length, StringComparison.Ordinal);
            }
        }

        /// <summary>
        ///   Checks if the content of two 2d <see cref="char" /> arrays are equal
        /// </summary>
        /// <param name="first">First array</param>
        /// <param name="second">Second array</param>
        /// <returns>
        ///   <see cref="bool" />
        /// </returns>
        internal static bool EqualityCheck(char[][] first, char[][] second)
        {
            return first[0].SequenceEqual(second[0]) &&
                   first[1].SequenceEqual(second[1]) &&
                   first[2].SequenceEqual(second[2]) &&
                   first[3].SequenceEqual(second[3]);
        }

        /// <summary>
        ///   Converts a 2d Character array to an array of strings
        /// </summary>
        /// <param name="chars">The char array to convert</param>
        /// <returns></returns>
        internal static string[] CharsToStrings(char[][] chars)
        {
            var res = new string[chars.Length];
            for (var i = 0; i < chars.Length; i++) res[i] = chars[i].ToString();
            return res;
        }

        /// <summary>
        ///   Writes out Character Arrays to the console
        /// </summary>
        internal static void DisplayRaw(char[][] chars)
        {
            foreach (var t in chars) Console.WriteLine(new string(t));
        }
    }
}