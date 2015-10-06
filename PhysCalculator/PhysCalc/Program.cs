﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing; 

using ConsolAnyColor;
using CommandParser;


// Assembly marked as compliant to CLS.
// [assembly: CLSCompliant(true)]  Using sbyte is not CLS-compliant

namespace PhysicalCalculator
{
    class Program
    {
        public static void Main(string[] args)
        {
           

            String EnvironmentCommandLine = Environment.CommandLine;


            DateTime now = DateTime.Now;
            TimeSpan TimeSince_2000_01_01 = now.Date - new DateTime(2000, 1, 1);
            int nowBuildNo = TimeSince_2000_01_01.Days;
            int nowRevisionNo = (int)(now.TimeOfDay.TotalSeconds / 2);

            var build = Environment.GetEnvironmentVariable("BUILD_NUMBER");
            build = build == null ? nowBuildNo.ToString() : int.Parse(build).ToString();
            var revision = Environment.GetEnvironmentVariable("REVISION");
            revision = revision == null ? nowRevisionNo.ToString() : int.Parse(revision).ToString();



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
                    ResultLineWriter.WriteErrorLine(String.Format("PhysCalculator failed to load with {0} arguments: \"{1}\" ", args.Count(), args.ToString()));
                }
                else
                {
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
