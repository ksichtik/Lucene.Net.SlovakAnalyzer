using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;
using Lucene.Net.Analysis.Hunspell;
using System.Text;

namespace SlovakAnalyzer
{
    public class SlovakNounAnalyzer : StandardAnalyzer
    {
        public SlovakNounAnalyzer() : base(Version.LUCENE_30, new StringReader(Properties.Resources.sk_SK_stopwords))
        {

        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            TokenStream stream = base.TokenStream(fieldName, reader);
            stream = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(Version.LUCENE_30), stream, STOP_WORDS_SET);
            stream = new SlovakNounFilter(stream);
            stream = new LowerCaseFilter(stream);
            stream = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(Version.LUCENE_30), stream, STOP_WORDS_SET);

            return stream;
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