using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvToPtt
{
    class ColoredString
    {
        public const char ASCII_ESC = (char)27;
        public enum ColorCode
        {
            NoColor,
            Red,
            Green,
        }

        public string Str { get; }
        public ColorCode Color { get; }
        public int Width { get; }
        public ColoredString(string str, ColorCode color)
        {
            Str = str;
            Color = color;

            if(str == null)
            {
                throw new ArgumentNullException(paramName: Str, message: "Why are you giving me null string?");
            }

            Width = GetOccupiedSpace(Str);
        }

        public string ToUncoloredString()
        {
            return this.Str;
        }
        public string ToColoredString()
        {
            return Color switch
            {
                ColorCode.NoColor => this.Str,
                ColorCode.Red => ASCII_ESC + "[1;31m" + ASCII_ESC + ' ' + this.Str + ASCII_ESC + "[m",
                ColorCode.Green => ASCII_ESC + "[1;32m" + ASCII_ESC + ' ' + this.Str + ASCII_ESC + "[m",
                _ => throw new NotImplementedException(message: $"ColorCode {Color} not implemented."),
            };
        }

        public static int GetOccupiedSpace(string str)
        {
            int occupiedSpace = 0;
            foreach (char c in str)
            {
                // Unicode character occupy 2 spaces while ANSI character occupy 1 space
                occupiedSpace += IsUnicodeCharacter(c) ? 2 : 1;
            }
            return occupiedSpace;
        }
        public static bool IsUnicodeCharacter(char input)
        {
            const char MaxAsciiCode = (char)255;

            return input > MaxAsciiCode;
        }
    }
}
