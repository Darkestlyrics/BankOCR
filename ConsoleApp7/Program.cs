using System;
using System.IO;
using OCRReader.Classes;
using OCRReader.Helpers;

namespace OCRReader
{
    internal class Program
    {
        private static readonly char[][] BufferArr = new char[4][];
        private static readonly OcrEngine OcrEngine = new OcrEngine(80, 1);
        private static string[] _lineStrings;
        private static int _recordCount;
        private static int _currRecord;


        private static void Main(string[] args)
        {
            ReadFile(args.Length == 1 ? args[0] : @".\TestData.txt");

            while (_currRecord != _recordCount)
            {
                ReadRecord();
                CharArrayHelper.DisplayRaw(BufferArr); //Show the current Buffer
                var decoded = OcrEngine.Decode(BufferArr);
                Console.WriteLine($"Result: {decoded}");
            }

            Console.ReadKey();
        }

        /// <summary>
        ///   Read the File into LineStrings
        /// </summary>
        /// <param name="path">Path of the File</param>
        private static void ReadFile(string path)
        {
            _lineStrings = File.ReadAllLines(path);
            _recordCount = _lineStrings.Length / 4; //As all Records are 4 lines in length
        }

        /// <summary>
        ///   Read the next record into bufferArr
        /// </summary>
        private static void ReadRecord()
        {
            var j = 0;
            var x = _currRecord == 0 ? 0 : _currRecord * 4;
            for (var i = x; i < x + 4; i++) //will always be 4 lines
            {
                BufferArr[j] =
                    _lineStrings[i].Length == 27
                        ? _lineStrings[i].ToCharArray()
                        : CharArrayHelper.PadArray(27); //Convert line to Character array in BufferArr
                j++;
            }

            _currRecord++; //Increment the Record Count
        }
    }
}