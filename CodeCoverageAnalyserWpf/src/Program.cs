using System;
using System.Numerics;
namespace CodeCoverageAnalyze;
class Program {
   public static void Main (string[] args) {
      Console.WriteLine ("Hello, World!");

      // Arrange
      var additions = new CodeCoverageAnalyze.AdditionsFolder.AdditionsClass (false);

      // Act
      int intResult = additions.AddInt (3, 4);
      string strResult = additions.AddStrings ("First Part ", "+ Second Part");
      Complex c1 = new Complex (1, 1); Complex c2 = new Complex (2, 2);
      var comxRes = additions.AddComplex (c1, c2);

      var multiplications = new CodeCoverageAnalyze.MultiplyFolder.MultiplyClass (true);
      int multResults = multiplications.MultiplyInts (4, 5);
      var multStrResults = multiplications.MultiplyStrings ("ExecFirstBlock", 3);

      if (DateTime.Now.Hour < 12)
         Console.WriteLine ("""
            Hello World!
            Good Morning.
            """);
      else
         Console.WriteLine ("""
            Hello World!
            Good afternoon.
            """);

   }
}

