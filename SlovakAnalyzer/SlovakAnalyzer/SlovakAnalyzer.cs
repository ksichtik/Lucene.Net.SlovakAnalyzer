using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Analysis.Hunspell;
using System.Text;
using System.Collections.Generic;
using Lucene.Net.Analysis.Tokenattributes;

namespace SlovakAnalyzer
{
    public class SlovakAnalyzer : StandardAnalyzer
    {
        public SlovakAnalyzer() : base(Version.LUCENE_30, new StringReader(Properties.Resources.sk_SK_stopwords))
        {

        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            TokenStream stream = base.TokenStream(fieldName, reader);
            stream = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(Version.LUCENE_30), stream, STOP_WORDS_SET);
            using (var affixStream = GenerateStreamFromString(Encoding.UTF8.GetString(Properties.Resources.sk_SK_aff)))
            using (var dictionaryStream = GenerateStreamFromString(Properties.Resources.sk_SK_dic))
            {
                var dict = new HunspellDictionary(affixStream, dictionaryStream);
                stream = new HunspellStemFilter(stream, dict);
            }
            stream = new LowerCaseFilter(stream);
            stream = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(Version.LUCENE_30), stream, STOP_WORDS_SET);   

            return stream;
        }

        public string ParseQueryString(string queryString)
        {
            List<string> tokens = new List<string>();
            var tokenStream = TokenStream(null, new StringReader(queryString));
            var offsetAttribute = tokenStream.GetAttribute<IOffsetAttribute>();
            var termAttribute = tokenStream.GetAttribute<ITermAttribute>();

            tokenStream.Reset();
            while (tokenStream.IncrementToken())
            {
                int startOffset = offsetAttribute.StartOffset;
                int endOffset = offsetAttribute.EndOffset;
                string term = termAttribute.Term;
                tokens.Add(term);
            }

            return string.Join(" ", tokens);
        }

        private static Stream GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            stream.Position = 0;
            return stream;
        }
    }
}