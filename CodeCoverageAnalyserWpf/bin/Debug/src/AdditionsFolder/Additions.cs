using System.Numerics;
namespace CodeCoverageAnalyze.AdditionsFolder;

public class AdditionsClass {
   public AdditionsClass( bool exeFirstBlock) { execFirstBlock = exeFirstBlock; }
   bool execFirstBlock = false;
   public int AddInt (int inta, int intb) {
      if (execFirstBlock) return inta + intb; else return intb + inta;
   }
   public string AddStrings (string strA, string strB) { if (strA == "ExecFirstBlock") return strA + strB; else return "Second Block Executed"; }
   public Complex AddComplex (Complex cmpxA, Complex cmpxB) => cmpxA + cmpxB;
}

