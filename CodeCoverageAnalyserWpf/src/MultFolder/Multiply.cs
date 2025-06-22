using System.Numerics;
namespace CodeCoverageAnalyze.MultiplyFolder;

public class MultiplyClass {
   public MultiplyClass (bool exeFirstBlock) { execFirstBlock = exeFirstBlock; }
   bool execFirstBlock = false;
   public int MultiplyInts (int inta, int intb) {
      if (execFirstBlock) return inta * intb; else return inta * intb * inta;
   }
   public string MultiplyStrings (string strA, int count) {
      if (strA == "ExecFirstBlock") return ""; else return new string (Enumerable.Repeat (strA, count).SelectMany (s => s).ToArray ());
   }
   public Complex MultiplyComplex (Complex cmpxA, Complex cmpxB) => cmpxA * cmpxB;
}
