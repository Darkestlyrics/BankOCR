using System;
using System.Collections.Generic;
using System.Linq;
using OCRReader.Enums;

namespace OCRReader.Helpers
{
    internal static class ConsoleHelper
    {
        private static readonly Dictionary<MessageType, ConsoleColor> TypeColors =
            new Dictionary<MessageType, ConsoleColor>
            {
                {MessageType.Info, ConsoleColor.Gray},
                {MessageType.Debug, ConsoleColor.Blue},
                {MessageType.Warning, ConsoleColor.Yellow},
                {MessageType.Error, ConsoleColor.Red}
            };

        public static void Write(string s, MessageType type)
        {
            Console.ForegroundColor = TypeColors.FirstOrDefault(o => o.Key == type).Value;
            Console.WriteLine(s);
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}