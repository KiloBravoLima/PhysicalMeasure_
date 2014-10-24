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
        static string PhysicalExpressionEval(string txtExpression)
        {
            string Result = "Internal error";
            try
            {
                ResultWriter ResultLineWriter = new ResultWriter();
                Commandreader CommandLineReader = new Commandreader(txtExpression, ResultLineWriter);
                CommandLineReader.ReadFromConsoleWhenEmpty = true;
                if (CommandLineReader == null)
                {
                    ResultLineWriter.WriteErrorLine(String.Format("PhysCalculator Commandreader failed to load with {0} arguments: \"{1}\" ", 1, txtExpression));
                }
                else
                {
                    PhysCalculator Calculator = new PhysCalculator(CommandLineReader, ResultLineWriter);
                    if (Calculator == null)
                    {
                        ResultLineWriter.WriteErrorLine(String.Format("PhysCalculator failed to load with {0} arguments: \"{1}\" ", 1, txtExpression));
                    }
                    else
                    {
                        CommandLineReader.ReadFromConsoleWhenEmpty = false;
                        CommandLineReader.WriteCommandToResultWriter = false;
                        ResultLineWriter.ResultLines = new List<String>();
                        // ResultLineWriter.WriteLine("PhysCalculator ready ");
                        Calculator.Run();
                        //ResultLineWriter.WriteLine("PhysCalculator finished");

                        if (ResultLineWriter.ResultLines.Count > 0)
                        {   // Use 
                            Result = "";

                            ResultLineWriter.ResultLines.ForEach(ResultLine =>
                            {
                                // Console.WriteLine(name);
                                if (!String.IsNullOrEmpty(Result))
                                {
                                    Result += "\n";
                                }
                                Result += ResultLine;
                            }
                            );


                        }
                    }
                }

            }
            /* 
            catch (EvalException ex)
            {
                // Report expression error and move caret to error position
                Result = "Evaluation error : " + ex.Message;
            }
            */
            catch (Exception ex)
            {
                // Unknown error
                Result = "Unexpected error : " + ex.Message;
            }

            return Result;
        }


        static void ShowPhysicalMeasureEval(ResultWriter ResultLineWriter, string str)
        {
            string res = /* PhysicalMeasure */ PhysicalExpressionEval(str);
            ResultLineWriter.WriteLine(str + " = " + res);
        }

        static void ShowStartLines(ResultWriter ResultLineWriter)
        {
            ResultLineWriter.WriteLine("c#:");
            
            ResultLineWriter.WriteLine("3 / 4 * 5 / 6 = " + (3D / 4 * 5 / 6).ToString() );
            ResultLineWriter.WriteLine("(3 / 4) * (5 / 6) = " + ((3D / 4) * (5D / 6)).ToString());
            ResultLineWriter.WriteLine("3 / 4 / 5 * 6 / 7 / 8 = " + (3D / 4 / 5 * 6D / 7 / 8).ToString());
            ResultLineWriter.WriteLine("((3 / 4) / 5) * ((6 / 7) / 8) = " + (((3D / 4) / 5) * ((6D / 7) / 8)).ToString());

            ResultLineWriter.WriteLine("3 - -2 = " + (3 - -2).ToString());
            ResultLineWriter.WriteLine("3 - - -2 = " + (3 - - -2).ToString());
            ResultLineWriter.WriteLine("3 - - - -2 = " + (3 - - - -2).ToString());
            ResultLineWriter.WriteLine("3 - - - + -2 = " + (3 - - -+-2).ToString());
            ResultLineWriter.WriteLine("3 - - - + + -2 = " + (3 - - -+ +-2).ToString());

            ResultLineWriter.WriteLine("PhysicalMeasure:");

            ShowPhysicalMeasureEval(ResultLineWriter, "3 / 4 * 5 / 6");
            ShowPhysicalMeasureEval(ResultLineWriter, "(3 / 4) * (5 / 6)");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 / 4 / 5 * 6 / 7 / 8");
            ShowPhysicalMeasureEval(ResultLineWriter, "((3 / 4) / 5) * ((6 / 7) / 8)");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 + -2");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 - -2");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 - - -2");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 - - - -2");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 - - - + -2");
            ShowPhysicalMeasureEval(ResultLineWriter, "3 - - - + + -2");

        }

        public static void Main(string[] args)
        {
            ConsolAnyColorClass.SetColor(ConsoleColor.Blue, Color.FromArgb(50, 50, 255));  // Slightliy light blue

            ResultWriter ResultLineWriter = new ResultWriter();

            //  ShowStartLines(ResultLineWriter);

            Commandreader CommandLineReader = new Commandreader(args, ResultLineWriter);
            CommandLineReader.ReadFromConsoleWhenEmpty = true;
#if DEBUG // Unit tests only included in debug build 
            if (System.Reflection.Assembly.GetEntryAssembly() == null)    
            { 
                // Do some setup to avoid error    
                // We want the test to run only the commands in the args
                CommandLineReader.ReadFromConsoleWhenEmpty = false;
            }
#endif 
            if (CommandLineReader == null)
            {
                ResultLineWriter.WriteErrorLine(String.Format("PhysCalculator Commandreader failed to load with {0} arguments: \"{1}\" ", args.Count(), args.ToString()));
            }
            else
            {
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
