namespace CodeCoverageAnalyserWpf
{
    public class CoverageRangeInfo
    {
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public bool IsCovered { get; set; }
        public int StartColumn { get; set; }
        public int EndColumn { get; set; }
    }
}
