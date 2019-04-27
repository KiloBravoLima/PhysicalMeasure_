using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using ConsolAnyColor;


// Assembly marked as compliant to CLS.
// [assembly: CLSCompliant(true)]  Using sbyte is not CLS-compliant

namespace PhysicalCalculator
{
    class Program
    {

        /*
            Start options:
                Command Line arguments: "//read ShortenFractions" "// read AllTestFiles" "// expression()" "include EpotFunc"

                Special test 1 
                Command Line arguments: "E:\Alle Brugere\Klaus\Programmering\Git_Repositories\KiloBravoLima\2019\PhysicalMeasure_\PhysCalculator\TestResults\Klaus_SORTE-PC 2019-04-22 19_59_29\Out\unittest_1.cal"

        */
        public static void Main(string[] args)
        {
            ConsolAnyColorClass.SetColor(ConsoleColor.Blue, Color.FromArgb(50, 50, 255));  // Slightly light blue

            ResultWriter ResultLineWriter = new ResultWriter();

            CommandReader CommandLineReader = new CommandReader(args, ResultLineWriter);
            if (CommandLineReader == null)
            {
                ResultLineWriter.WriteErrorLine(String.Format("PhysCalculator CommandReader failed to load with {0} arguments: \"{1}\" ", args.Count(), args.ToString()));
            }
            else
            {
                CommandLineReader.ReadFromConsoleWhenEmpty = true;
#if DEBUG // Unit tests only included in debug build 
                if (System.Reflection.Assembly.GetEntryAssembly() == null)
                {
                    // Do some setup to avoid error    
                    // We want the test to run only the commands in the args
                    CommandLineReader.ReadFromConsoleWhenEmpty = false;
                }
#endif 

                PhysCalculator Calculator = new PhysCalculator(CommandLineReader, ResultLineWriter);
                if (Calculator == null)
                {
                    ResultLineWriter.WriteErrorLine($"PhysCalculator failed to load with {args.Count()} arguments: \"{args.ToString()}\" ");
                }
                else
                {

// #if DEBUG 
                    ResultLineWriter.WriteErrorLine($"PhysCalculator called with {args.Count()} arguments: \"{args.ToStringList()}\" ");
// #endif 

                    ResultLineWriter.ForegroundColor = ConsoleColor.Blue;
                    ResultLineWriter.WriteLine("PhysCalculator ready");
                    ResultLineWriter.ResetColor();

                    Calculator.Run();

                    ResultLineWriter.ForegroundColor = ConsoleColor.Blue;
                    ResultLineWriter.WriteLine("PhysCalculator finished");
                }
            }
        }
    }
}
