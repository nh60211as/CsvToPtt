using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Microsoft.VisualBasic.FileIO;
using System.IO;
using System.Linq;

namespace CsvToPtt
{
    class CsvTable : DataTable
    {
        public const int PttMaxWidth = 80; // max halfwidth characters to be displayed on PTT
        public readonly string PCManNewLine = "\r\n"; // PCMan only accept \r\n new line

        private enum Justify
        {
            Left,
            Right
        }

        public bool HasHeader { get; }
        public List<ColoredString> HeaderStrings { get; } = new List<ColoredString>();
        public int FieldSpace { get; }
        private readonly string FieldSpaceString;

        public CsvTable(string csvPath, bool hasHeader = true, int fieldSpace = 2)
        {
            HasHeader = hasHeader;
            FieldSpace = fieldSpace;

            FieldSpaceString = new string(' ', FieldSpace);

            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException(message: $"CSV file {csvPath} does not exist.", fileName: csvPath);
            }

            StreamReader stream = new StreamReader(csvPath,Encoding.UTF8);
            TextFieldParser reader = new TextFieldParser(stream)
            {
                HasFieldsEnclosedInQuotes = true,
                Delimiters = new string[] { "," },
                CommentTokens = Array.Empty<string>(),
                TrimWhiteSpace = false,
                TextFieldType = FieldType.Delimited,
            };

            if (HasHeader)
            {
                string[] headerBlock = reader.ReadFields();

                foreach (string header in headerBlock)
                {
                    HeaderStrings.Add(new ColoredString(header, ColoredString.ColorCode.NoColor));

                    this.Columns.Add(header, typeof(ColoredString));
                }
            }
            else
            {
                string[] headerBlock = reader.ReadFields();

                foreach (string header in headerBlock)
                {
                    HeaderStrings.Add(new ColoredString("", ColoredString.ColorCode.NoColor));

                    this.Columns.Add("", typeof(ColoredString));
                }

                // rewind to starting point of the stream
                stream.BaseStream.Position = 0;
                stream.DiscardBufferedData();
            }

            while (!reader.EndOfData)
            {
                string[] entryBlock = reader.ReadFields();

                DataRow row = this.NewRow();
                for(int i=0;i<entryBlock.Length;i++)
                {
                    row[i] = new ColoredString(entryBlock[i], ColoredString.ColorCode.NoColor);
                }
                this.Rows.Add(row);
            }
        }

        public override string ToString()
        {
            List<int> columnsOccupiedSpaceList = GetAllColumnsOccupiedSpace();

            StringBuilder stringBuilder = new StringBuilder();

            if(HasHeader)
            {
                stringBuilder.Append(HeaderString(preserveSpace: 0, columnsOccupiedSpaceList));
                stringBuilder.Append(PCManNewLine);
            }

            // only the first column is left justified
            List<Justify> justifyList = Enumerable.Repeat(Justify.Right, HeaderStrings.Count).ToList();
            justifyList[0] = Justify.Left;

            foreach (DataRow row in this.Rows)
            {
                stringBuilder.Append(EntryString(row, preserveSpace: 0, columnsOccupiedSpaceList, justifyList));
                stringBuilder.Append(PCManNewLine);
            }

            return stringBuilder.ToString();
        }

        private List<int> GetAllColumnsOccupiedSpace()
        {
            List<int> ans = new List<int>(HeaderStrings.Count); // initialize the capacity, not the size
            for (int i = 0; i < HeaderStrings.Count; i++)
            {
                ans.Add(GetColumnOccupiedSpace(i));
            }

            return ans;
        }

        private int GetColumnOccupiedSpace(int index)
        {
            int maxOccupiedSpace = HeaderStrings[index].Width;

            foreach (DataRow row in this.Rows)
            {
                ColoredString currentString = (ColoredString)row[index];

                int currentOccupiedSpace = currentString.Width;

                if (currentOccupiedSpace > maxOccupiedSpace)
                    maxOccupiedSpace = currentOccupiedSpace;
            }

            return maxOccupiedSpace;
        }

        private string HeaderString(int preserveSpace, List<int> columnsOccupiedSpaceList)
        {
            string ans = "";

            if(preserveSpace > 0)
            {
                ans += new string(' ', preserveSpace);
                ans += FieldSpaceString;
            }

            for (int i=0;i< HeaderStrings.Count;i++)
            {
                ans += GetUnicodeAwarePaddedString(HeaderStrings[i], columnsOccupiedSpaceList[i], Justify.Left);

                if(i!= HeaderStrings.Count-1)
                    ans += FieldSpaceString;
            };

            return ans;
        }
        private string EntryString(DataRow row, int preserveSpace, List<int> columnsOccupiedSpaceList, List<Justify> justifyList)
        {
            string ans = "";

            if (preserveSpace > 0)
            {
                ans += new string(' ', preserveSpace);
                ans += FieldSpaceString;
            }

            for (int i = 0; i < HeaderStrings.Count; i++)
            {
                ans += GetUnicodeAwarePaddedString((ColoredString)row[i], columnsOccupiedSpaceList[i], justifyList[i]);

                if (i != HeaderStrings.Count - 1)
                    ans += FieldSpaceString;
            };

            return ans;
        }

        private static string GetUnicodeAwarePaddedString(ColoredString coloredString, int space, Justify justify)
        {
            int actualSpace = Math.Max(space - coloredString.Width, 0);
            return justify switch
            {
                Justify.Left => coloredString.ToColoredString() + new string(' ', actualSpace),
                Justify.Right => new string(' ', actualSpace) + coloredString.ToColoredString(),
                _ => throw new NotImplementedException(message: $"Justify {justify} not implemented."),
            };
        }
    }
}
