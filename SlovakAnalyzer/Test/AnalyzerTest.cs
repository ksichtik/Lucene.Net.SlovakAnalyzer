using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SLovakAnalyzerTest
{
    [TestClass]
    public class AnalyzerTest
    {
        [TestMethod]
        public void Slovak_analyzer_test()
        {
            var inputString = "Keďže poľnohospodári Finstatu, Chirany, Lega, U. S. Steelu Košice a dubu z finstat.sk v Zambii preferujú skôr všestranné a spoľahlivé traktory s ľahkou údržbou...";
            var expectedString = "keďže poľnohospodár finstat chiran lego u steel košic dub finstat.sk zamb preferovať skôr všestranný spoľahlivý traktor ľahký údržba";
            List<string> tokens = new List<string>();
            var analyzer = new SlovakAnalyzer.SlovakAnalyzer();
            var tokenStream = analyzer.TokenStream(null, new StringReader(inputString));
            var offsetAttribute = tokenStream.GetAttribute<IOffsetAttribute>();
            var termAttribute = tokenStream.GetAttribute<ITermAttribute>();

            tokenStream.Reset();
            while (tokenStream.IncrementToken())
            {
                int startOffset = offsetAttribute.StartOffset;
                int endOffset = offsetAttribute.EndOffset;
                String term = termAttribute.Term;
                tokens.Add(term);
            }

            Assert.AreEqual(expectedString, string.Join(" ", tokens));
        }

        [TestMethod]
        public void Slovak_noun_analyzer_test()
        {
            var inputString = "Keďže najväčšia poľnohospodári Finstatu, Chirany, Lega, U. S. Steelu Košice a dubu z finstat.sk v Zambii preferujú skôr všestranné a spoľahlivé traktory s ľahkou údržbou...";
            var expectedString = "keďž väčš poľnohospodár finstat chiran leg u steel košic dub finstat.sk zamb preferuj skor všestrann spoľahliv traktor ľahk údržb";
            List<string> tokens = new List<string>();
            var analyzer = new SlovakAnalyzer.SlovakNounAnalyzer();
            var tokenStream = analyzer.TokenStream(null, new StringReader(inputString));
            var offsetAttribute = tokenStream.GetAttribute<IOffsetAttribute>();
            var termAttribute = tokenStream.GetAttribute<ITermAttribute>();

            tokenStream.Reset();
            while (tokenStream.IncrementToken())
            {
                int startOffset = offsetAttribute.StartOffset;
                int endOffset = offsetAttribute.EndOffset;
                String term = termAttribute.Term;
                tokens.Add(term);
            }

            Assert.AreEqual(expectedString, string.Join(" ", tokens));
        }
    }
}