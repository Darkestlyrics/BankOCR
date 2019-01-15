using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OCRReader.Helpers;

namespace OCRReader.Classes
{
    internal class OcrEngine
    {
        private readonly int _diceThreshold;
        private readonly int _hammingThreshold;
        private readonly int _misreadThreshold;

        /// <summary>
        ///   The Dictionary of characters
        /// </summary>
        private readonly Dictionary<char, char[][]> _ocrDict = new Dictionary<char, char[][]>
        {
            {
                '0',
                new[]
                {
                    new[] {' ', '_', ' '},
                    new[] {'|', ' ', '|'},
                    new[] {'|', '_', '|'},
                    new[] {' ', ' ', ' '}
                }
            },
            {
                '1',
                new[]
                {
                    new[] {' ', ' ', ' '},
                    new[] {' ', ' ', '|'},
                    new[] {' ', ' ', '|'},
                    new[] {' ', ' ', ' '}
                }
            },
            {
                '2',
                new[]
                {
                    new[] {' ', '_', ' '},
                    new[] {' ', '_', '|'},
                    new[] {'|', '_', ' '},
                    new[] {' ', ' ', ' '}
                }
            },
            {
                '3',
                new[]
                {
                    new[] {' ', '_', ' '},
                    new[] {' ', '_', '|'},
                    new[] {' ', '_', '|'},
                    new[] {' ', ' ', ' '}
                }
            },
            {
                '4',
                new[]
                {
                    new[] {' ', ' ', ' '},
                    new[] {'|', '_', '|'},
                    new[] {' ', ' ', '|'},
                    new[] {' ', ' ', ' '}
                }
            },
            {
                '5',
                new[]
                {
                    new[] {' ', '_', ' '},
                    new[] {'|', '_', ' '},
                    new[] {' ', '_', '|'},
                    new[] {' ', ' ', ' '}
                }
            },
            {
                '6',
                new[]
                {
                    new[] {' ', '_', ' '},
                    new[] {'|', '_', ' '},
                    new[] {'|', '_', '|'},
                    new[] {' ', ' ', ' '}
                }
            },
            {
                '7',
                new[]
                {
                    new[] {' ', '_', ' '},
                    new[] {' ', ' ', '|'},
                    new[] {' ', ' ', '|'},
                    new[] {' ', ' ', ' '}
                }
            },
            {
                '8',
                new[]
                {
                    new[] {' ', '_', ' '},
                    new[] {'|', '_', '|'},
                    new[] {'|', '_', '|'},
                    new[] {' ', ' ', ' '}
                }
            },
            {
                '9',
                new[]
                {
                    new[] {' ', '_', ' '},
                    new[] {'|', '_', '|'},
                    new[] {' ', '_', '|'},
                    new[] {' ', ' ', ' '}
                }
            }
        };


        public OcrEngine()
        {
            _diceThreshold = 80;
            _hammingThreshold = 1;
            _misreadThreshold = 3;
        }

        public OcrEngine(int diceThreshold, int hammingThreshold, int misreadThreshold)
        {
            _diceThreshold = diceThreshold;
            _hammingThreshold = hammingThreshold;
            _misreadThreshold = misreadThreshold;
        }

        /// <summary>
        ///   Decodes a line of characters
        /// </summary>
        /// <param name="record">The line to decode</param>
        /// <returns><see cref="string" />The decoded String</returns>
        public string Decode(char[][] record)
        {
            var res = new StringBuilder();
            char[][] temp;
            for (var i = 0; i < record[0].Length / 3; i++)
            {
                temp = CharArrayHelper.GetCharAtIndex(record, i);
                var match = GetMatch(temp);
                res.Append(match);
            }

            var decoded = res.ToString();
            res.Clear();

            if (decoded.Contains("?"))
            {
                var misreadCount = decoded.ToCharArray().Count(o => o == '?');
                if (misreadCount >= _misreadThreshold)
                    res.AppendLine($"Decoded string misread count reached misread threshold: {decoded}");
                res.AppendLine(
                    $"Decoded string contains {misreadCount} misread character(s): {decoded} \r\n Attempting prediction");
                var possibleStrings = GetPossibleStrings(record, decoded);
                foreach (var possibleString in possibleStrings)
                {
                    res.AppendLine($"Possible Decoded String: {possibleString}");
                    res.AppendLine($"Check sum Result: {(Checksum(possibleString) ? " Pass" : " Fail")}");
                }
            }
            else
            {
                res.AppendLine($"Result: {decoded}");
                res.AppendLine($"Check sum Result: {(Checksum(decoded) ? " Pass" : " Fail")}");
            }

            return res.ToString();
        }

        /// <summary>
        ///   Gets all possible strings from a string with a misread character
        /// </summary>
        /// <param name="record"></param>
        /// <param name="decoded"></param>
        /// <returns></returns>
        private List<string> GetPossibleStrings(char[][] record, string decoded)
        {
            var res = new List<string>();
            var initialPostion = decoded.IndexOf("?", StringComparison.Ordinal);
            var tempString = decoded;
            var checkChar = CharArrayHelper.GetCharAtIndex(record, initialPostion);
            var possibleChars = GetPossibleMatches(checkChar);
            foreach (var possibleChar in possibleChars)
            {
                tempString = tempString.Remove(initialPostion, 1).Insert(initialPostion, possibleChar.ToString());
                if (tempString.Contains("?"))
                    GetPossibleStrings(record, tempString).ForEach(o =>
                        res.Add(o));
                else
                    res.Add(tempString);
            }

            return res;
        }


        /// <summary>
        /// </summary>
        /// <param name="temp"></param>
        /// <returns></returns>
        private List<char> GetPossibleMatches(char[][] temp)
        {
            List<char> possibleMatches;
            var possibleDictionary = CheckPossible(temp);
            possibleMatches = possibleDictionary.Where(o => o.Value.DiceCoeff >= _diceThreshold)
                .Where(x => x.Value.Hamming <= _hammingThreshold)
                .Select(o => o.Key)
                .ToList();
            return possibleMatches;
        }

        /// <summary>
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        private char GetMatch(char[][] num)
        {
            var res = '?'; //Return Question mark if none found
            if (_ocrDict.Values.Any(o => CharArrayHelper.EqualityCheck(num, o)))
                res = _ocrDict.First(o => CharArrayHelper.EqualityCheck(num, o.Value)).Key;
            return res;
        }


        /// <summary>
        ///   Performs a checksum calculation on the number passed
        /// </summary>
        /// <param name="num">The string to perform the Checksum calculation on</param>
        /// <returns><see cref="bool" />Pass or fail</returns>
        private bool Checksum(string num)
        {
            if (num.Contains("?")) return false;
            return (num[0] + num[1] * 2 + num[2] * 4 + num[3] * 5 + num[4] * 6 + num[5] * 7 + num[6] * 8 +
                    num[7] * 9 + num[8]) % 11 == 0;
        }

        /// <summary>
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        private Dictionary<char, ConfidenceResult> CheckPossible(char[][] input)
        {
            var confidenceDictionary = new Dictionary<char, ConfidenceResult>();
            foreach (var pair in _ocrDict) confidenceDictionary.Add(pair.Key, CalcDistance(input, pair.Value));

            return confidenceDictionary;
        }

        /// <summary>
        ///   Calculates the distance between two collections
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns></returns>
        private ConfidenceResult CalcDistance(char[][] first, char[][] second)
        {
            var res = new ConfidenceResult();
            var string1 = CharArrayHelper.ArrToString(first);
            var string2 = CharArrayHelper.ArrToString(second);
            res.DiceCoeff = (int) Math.Round(DiceMatch(string1, string2) * 100, 0);
            res.Hamming = GetHammingDistance(string1, string2);
            return res;
        }

        /// <summary>
        ///   Get the DiceCoefficient of two strings
        /// </summary>
        /// <param name="string1">The first String</param>
        /// <param name="string2">The second string</param>
        /// <returns>
        ///   <see cref="double" />
        /// </returns>
        private double DiceMatch(string string1, string string2)
        {
            if (string.IsNullOrEmpty(string1) || string.IsNullOrEmpty(string2))
                return 0;

            if (string1 == string2)
                return 1;

            var strlen1 = string1.Length;
            var strlen2 = string2.Length;

            if (strlen1 < 2 || strlen2 < 2)
                return 0;

            var length1 = strlen1 - 1;
            var length2 = strlen2 - 1;

            double matches = 0;
            var i = 0;
            var j = 0;

            while (i < length1 && j < length2)
            {
                var a = string1.Substring(i, 2);
                var b = string2.Substring(j, 2);
                var cmp = string.CompareOrdinal(a, b);

                if (cmp == 0)
                    matches += 2;

                ++i;
                ++j;
            }

            return matches / (length1 + length2);
        }

        /// <summary>
        ///   Get the Hamming Distance between two strings
        /// </summary>
        /// <param name="s">The first String</param>
        /// <param name="t">The second string</param>
        /// <returns>
        ///   <see cref="int" />
        /// </returns>
        private int GetHammingDistance(string s, string t)
        {
            if (s.Length != t.Length) throw new Exception("Strings must be equal length");

            var distance =
                s.ToCharArray()
                    .Zip(t.ToCharArray(), (c1, c2) => new {c1, c2})
                    .Count(m => m.c1 != m.c2);

            return distance;
        }
    }
}