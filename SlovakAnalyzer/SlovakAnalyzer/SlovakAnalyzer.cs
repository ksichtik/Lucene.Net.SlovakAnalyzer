using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Analysis.Hunspell;
using System.Text;
using System.Collections.Generic;
using Lucene.Net.Analysis.Tokenattributes;
using System.Linq;

namespace SlovakAnalyzer
{
    public class SlovakAnalyzer : StandardAnalyzer
    {
        /// <summary>
        /// Cached hunspell dictionary
        /// </summary>
        private static HunspellDictionary _hunspellDictionary;
        /// <summary>
        /// Static constructor to load the hunspell dictionary from resources
        /// </summary>
        static SlovakAnalyzer()
        {
            _hunspellDictionary = LoadHunspellDictionaryFromResources();
        }

        public SlovakAnalyzer() : base(Version.LUCENE_30, new HashSet<string>() /*, new StringReader(Properties.Resources.sk_SK_stopwords)*/)
        {
            
        }

        private static HunspellDictionary LoadHunspellDictionaryFromResources()
        {
            using (var affixStream = GenerateStreamFromString(Encoding.UTF8.GetString(Properties.Resources.sk_SK_aff)))
            using (var dictionaryStream = GenerateStreamFromString(Properties.Resources.sk_SK_dic))
            {
                return new HunspellDictionary(affixStream, dictionaryStream);
            }
        }
        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            TokenStream stream = base.TokenStream(fieldName, reader);

            return new HunspellStemFilter(stream, _hunspellDictionary); ;
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

            var resultString =  string.Join(" ", tokens);
            return new string(resultString.Where(c => !char.IsControl(c)).ToArray());
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