using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Test
{
    [TestClass]
    public class AnalyzerTest
    {
        [TestMethod]
        public void Slova_analyzer_test()
        {
            var inputString = "Keďže poľnohospodári Finstatu z finstat.sk v Zambii preferujú skôr všestranné a spoľahlivé traktory s ľahkou údržbou...";
            var expectedString = "keďže poľnohospodár finstatu finstat.sk zambii preferovať skôr všestranný spoľahlivý traktor ľahký údržba";
            List<string> tokens = new List<string>();
            var analyzer = new SlovakAnalyzer.SlovakAnalyzer();
            var tokenStream = analyzer.TokenStream(null, new StringReader(inputString));
            var offsetAttribute = (OffsetAttribute)tokenStream.GetAttribute(typeof(OffsetAttribute));
            var termAttribute = (TermAttribute)tokenStream.GetAttribute(typeof(TermAttribute));

            tokenStream.Reset();
            while (tokenStream.IncrementToken())
            {
                int startOffset = offsetAttribute.StartOffset();
                int endOffset = offsetAttribute.EndOffset();
                String term = termAttribute.Term();
                tokens.Add(term);
            }

            Assert.AreEqual(expectedString, string.Join(" ", tokens));
        }
    }
}