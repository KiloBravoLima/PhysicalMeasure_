using System;
using System.Linq;

namespace PhysCalc
{
    class Program
    {
        static void Main(string[] args)
        {
            ResultWriter ConsoleLineWriter = new ResultWriter();
            Commandreader PhysCommands = new Commandreader(args, ConsoleLineWriter);
            if (PhysCommands == null)
            {
                ConsoleLineWriter.WriteLine(String.Format("PhysCalculator Commandreader failed to load with {0} arguments: \"{1}\" ", args.Count(), args.ToString()));
            }
            else
            {
                PhysCalculator Calculator = new PhysCalculator(PhysCommands, ConsoleLineWriter);
                if (Calculator == null)
                {
                    Console.WriteLine(String.Format("PhysCalculator failed to load with {0} arguments: \"{1}\" ", args.Count(), args.ToString()));
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
