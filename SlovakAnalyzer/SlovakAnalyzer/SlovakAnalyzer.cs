using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Version = Lucene.Net.Util.Version;
using System.Collections.Generic;

namespace SlovakAnalyzer
{
    public class SlovakAnalyzer : StandardAnalyzer
    {
        public SlovakAnalyzer() : base(Version.LUCENE_30, SlovakStopWords)
        {

        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            TokenStream stream = base.TokenStream(fieldName, reader);
            stream = new LowerCaseFilter(stream);
            stream = new StopFilter(StopFilter.GetEnablePositionIncrementsVersionDefault(Version.LUCENE_30), stream, STOP_WORDS_SET);

            return stream;
        }

        private static HashSet<string> SlovakStopWords = new HashSet<string>(new string[] {
            "a",
            "aby",
            "aj",
            "ako",
            "ale",
            "alebo",
            "ani",
            "asi",
            "až",
            "áno",
            "bez",
            "buď",
            "by",
            "cez",
            "či",
            "čo",
            "ešte",
            "ho",
            "i",
            "iba",
            "ich",
            "ja",
            "je",
            "jeho",
            "jej",
            "ju",
            "k",
            "ku",
            "kam",
            "kde",
            "keď",
            "kto",
            "menej",
            "mi",
            "moja",
            "moje",
            "môj",
            "my",
            "nad",
            "nám",
            "než",
            "nič",
            "nie",
            "o",
            "od",
            "on",
            "ona",
            "oni",
            "ono",
            "po",
            "pod",
            "podľa",
            "pokiaľ",
            "potom",
            "práve",
            "prečo",
            "pred",
            "preto",
            "pretože",
            "pri",
            "s",
            "sa",
            "si",
            "sme",
            "so",
            "som",
            "späť",
            "ste",
            "sú",
            "ta",
            "tak",
            "takže",
            "tam",
            "tá",
            "táto",
            "teda",
            "ten",
            "tento",
            "tieto",
            "tiež",
            "to",
            "toho",
            "tom",
            "tomto",
            "toto",
            "tu",
            "túto",
            "ty",
            "tým",
            "týmto",
            "už",
            "v",
            "vám",
            "viac",
            "vo",
            "však",
            "vy",
            "z",
            "za",
            "zo",
        });
    }
}
