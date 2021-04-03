using System;
using System.IO;

namespace CsvToPtt
{
    class Program
    {
        static void Main(string[] args)
        {
            string inputPath = "";
            string outputPath = "";
            if (args.Length==0)
            {
                PrintHelp();
                return;
            }
            else if(args.Length==1)
            {
                inputPath = args[0];
                outputPath = "./output.txt";
            }
            else if (args.Length == 2)
            {
                inputPath = args[0];
                outputPath = args[1];
            }
            else
            {
                PrintHelp();
                return;
            }

            CsvTable csvTable = new CsvTable(inputPath,
                hasHeader: true,
                fieldSpace: 2);

            StreamWriter writer = new StreamWriter(outputPath);
            writer.Write(csvTable.ToString());
            writer.Close();
        }

        public static void PrintHelp()
        {
            Console.WriteLine("CSV to PTT helper 1.0.0");
            Console.WriteLine("Example input 1: CsvToPtt.exe \"input.csv\"");
            Console.WriteLine("This input will parse input.csv and output to output.txt\n");
            Console.WriteLine("Example input 2: CsvToPtt.exe \"input.csv\" \"myOutputPath.txt\"");
            Console.WriteLine("This input will parse input.csv and output to myOutputPath.txt\n");
        }
    }
}
