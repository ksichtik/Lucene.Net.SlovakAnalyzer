using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Hunspell;
using Lucene.Net.Analysis.Tokenattributes;

namespace SlovakAnalyzer
{
    public class SlovakNounFilter : TokenFilter
    {
        private readonly ITermAttribute termAtt;
        private readonly IOffsetAttribute posAtt;
        private readonly SlovakStemmer stemmer;

        public SlovakNounFilter(TokenStream input) : base(input)
        {
            termAtt = AddAttribute<ITermAttribute>();
            posAtt = AddAttribute<IOffsetAttribute>();
            stemmer = new SlovakStemmer();
        }

        public override bool IncrementToken()
        {
            if (input.IncrementToken())
            {
                var currentTerm = new string(termAtt.TermBuffer(), 0, termAtt.TermLength());
                if (!string.IsNullOrEmpty(currentTerm))
                {
                    stemmer.Stem(termAtt.TermBuffer(), termAtt.TermLength(), out char[] newTerm, out var newLength);
                    termAtt.SetTermBuffer(newTerm, 0, newLength);
                    termAtt.SetTermLength(newLength);
                }
                return true;
            }
            else
                return false;
        }
    }
}