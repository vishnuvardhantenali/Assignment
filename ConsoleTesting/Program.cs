using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace MaxProfitAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            int[] profits = 
            {
                100, -30, -120, 10, 2, 16, 12, 14, -11, 0,
                -121, 34, 0, -200, 24, 54, 43, 0, 31, -13
            };           

            int maximumSum = profits[0];
            int currentSum = profits[0];
            int start = 0, end = 0, tempStart = 0;

            for (int i = 1; i < profits.Length; i++)
            {
                int v = profits[i];

                if (currentSum < 0)
                {                    
                    currentSum = v;
                    tempStart = i;
                }
                else
                {
                    currentSum += v;
                }

                if (currentSum > maximumSum)
                {
                    maximumSum = currentSum;
                    start = tempStart;
                    end = i;
                }
            }

            Console.WriteLine ($"Maximum Profit: {maximumSum}");
            Console.WriteLine ($"Consecutive Days: Day {start + 1} to Day {end + 1}");

            Console.ReadLine ();
        }
    }
}
