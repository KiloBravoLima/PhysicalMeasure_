﻿using System;
using System.Linq;

using CommandParser;

namespace PhysicalCalculator
{
    class Program
    {
        static void Main(string[] args)
        {
            ResultWriter ConsoleLineWriter = new ResultWriter();
            Commandreader CommandLineReader = new Commandreader(args, ConsoleLineWriter);
            CommandLineReader.ReadFromConsoleWhenEmpty = true;
            if (CommandLineReader == null)
            {
                ConsoleLineWriter.WriteLine(String.Format("PhysCalculator Commandreader failed to load with {0} arguments: \"{1}\" ", args.Count(), args.ToString()));
            }
            else
            {
                PhysCalculator Calculator = new PhysCalculator(CommandLineReader, ConsoleLineWriter);
                if (Calculator == null)
                {
                    ConsoleLineWriter.WriteLine(String.Format("PhysCalculator failed to load with {0} arguments: \"{1}\" ", args.Count(), args.ToString()));
                }
                else
                {
                    ConsoleLineWriter.WriteLine("PhysCalculator ready");
                    Calculator.Run();
                    ConsoleLineWriter.WriteLine("PhysCalculator finished");
                }
            }
        }
    }
}
