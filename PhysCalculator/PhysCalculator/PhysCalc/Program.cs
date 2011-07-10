﻿using System;
using System.Linq;

using CommandParser;

namespace PhysicalCalculator
{
    class Program
    {
        static void Main(string[] args)
        {

            ResultWriter ResultLineWriter = new ResultWriter();
            Commandreader CommandLineReader = new Commandreader(args, ResultLineWriter);
            CommandLineReader.ReadFromConsoleWhenEmpty = true;
            if (CommandLineReader == null)
            {
                ResultLineWriter.WriteLine(String.Format("PhysCalculator Commandreader failed to load with {0} arguments: \"{1}\" ", args.Count(), args.ToString()));
            }
            else
            {
                PhysCalculator Calculator = new PhysCalculator(CommandLineReader, ResultLineWriter);
                if (Calculator == null)
                {
                    ResultLineWriter.WriteLine(String.Format("PhysCalculator failed to load with {0} arguments: \"{1}\" ", args.Count(), args.ToString()));
                }
                else
                {
                    ResultLineWriter.WriteLine("PhysCalculator ready");
                    Calculator.Run();
                    ResultLineWriter.WriteLine("PhysCalculator finished");
                }
            }
        }
    }
}
