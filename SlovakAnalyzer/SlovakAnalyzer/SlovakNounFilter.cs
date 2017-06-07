using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Util;

namespace SlovakAnalyzer
{
    public class SlovakNounFilter : TokenFilter
    {
        private readonly TermAttribute termAtt;
        private readonly OffsetAttribute posAtt;
        private readonly SlovakStemmer stemmer;

        public SlovakNounFilter(TokenStream input) : base(input)
        {
            termAtt = (TermAttribute)AddAttribute(typeof(TermAttribute));
            posAtt = (OffsetAttribute)AddAttribute(typeof(OffsetAttribute));
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