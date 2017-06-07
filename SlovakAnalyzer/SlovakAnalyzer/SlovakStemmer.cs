using Lucene.Net.Search.Vectorhighlight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SlovakAnalyzer
{
    public class SlovakStemmer
    {
        private static string[] Vowels = new string[] {"a", "á", "ä", "e", "é", "i", "ia", "ie", "iu", "í", "o", "ó", "u", "ú", "y", "ý", "ô"};
        private static string[] EI = new string[] { "e", "i", "ia", "iach", "iam", "iami", "ie", "iu", "í", "ím" };
        private static string[] LR = new string[] { "r", "ŕ", "l", "ĺ" };
        private static string[] ForeignWordsBeforeIA = new String[] { "c", "z", "g" };
        private static List<string[]> Suffixes = CreateSuffixes();
        private static Dictionary<string, string> DTNL = CreateDTNL();
        private static Dictionary<string, string> LongShort = CreateLongShort();

        private char[] Term { get; set; }
        private int TermLength { get; set; }

        public void Stem(char[] term, int length, out char[] stemmedTerm, out int stemmedLength)
        {
            Term = term;
            TermLength = length;
            AddSuffix(string.Empty);

            RemovePrefix();
            RemoveSuffix();

            stemmedTerm = Term;
            stemmedLength = TermLength;
        }

        #region Methods

        private void RemovePrefix()
        {
            if (TermLength > 6 && StartsWith("naj"))
            {
                RemovePart(0, 3);
            }
        }

        private void RemoveSuffix()
        {
            foreach(var suffixLevel in Suffixes)
            {
                foreach (var suffix in suffixLevel)
                {
                    if (EndsWith(suffix))
                    {
                        if (ContainsEI(suffix))
                        {
                            RemovePart(TermLength - suffix.Length, suffix.Length);
                            ChangeDTNL();
                            return;
                        }

                        if (suffix.StartsWith("i"))
                        {
                            if (Foreign(suffix))
                            {
                                RemovePart(TermLength - suffix.Length, suffix.Length);
                                AddSuffix("i");
                                return;
                            }
                        }

                        if (Overstemming(suffix))
                        {
                            return;
                        }

                        RemovePart(TermLength - suffix.Length, suffix.Length);
                        return;
                    }
                }
            }

            // er -> peter, sveter....
            var _suffix = "er";
            if (EndsWith(_suffix)) { 
                RemovePart(TermLength - _suffix.Length, _suffix.Length);
                AddSuffix("r");
                return;
            }

            //ok -> sviatok, odpadok....
            _suffix = "ok";
            if (EndsWith(_suffix))
            {
                RemovePart(TermLength - _suffix.Length, _suffix.Length);
                AddSuffix("k");
                return;
            }

            //zen -> podobizen, bielizen....
            _suffix = "zeň";
            if (EndsWith(_suffix))
            {
                _suffix = "eň";
                RemovePart(TermLength - _suffix.Length, _suffix.Length);
                AddSuffix("ň");
                return;
            }

            //na ol -> kotol....
            _suffix = "ol";
            if (EndsWith(_suffix))
            {
                RemovePart(TermLength - _suffix.Length, _suffix.Length);
                AddSuffix("l");
                return;
            }

            //ic -> matematic (matematik, matematici)... (pracovnici vs slnecnic)
            _suffix = "ic";
            if (EndsWith(_suffix))
            {
                _suffix = "c";
                RemovePart(TermLength - _suffix.Length, _suffix.Length);
                AddSuffix("k");
                return;
            }

            //ec -> tanec, obec....
            _suffix = "ec";
            if (EndsWith(_suffix))
            {
                RemovePart(TermLength - _suffix.Length, _suffix.Length);
                AddSuffix("c");
                return;
            }

            //um -> studium, stadium....
            _suffix = "um";
            if (EndsWith(_suffix))
            {
                RemovePart(TermLength - _suffix.Length, _suffix.Length);
                return;
            }

            GenitivePlural();
            return;
        }

        /// <summary>
        /// detenele, ditinili
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        private static bool ContainsEI(string suffix)
        {
            if (EI.Any(x => suffix.Equals(x)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// foreign words ending with -cia, -gia...
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        private bool Foreign(string suffix)
        {
            String s = TryRemovePart(Term.Length - suffix.Length, suffix.Length);
            if (ForeignWordsBeforeIA.Any(x => s.EndsWith(x)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// overstemming happends, when we have word root without a vowel / without l,r inside of word
        /// </summary>
        /// <param name="suffix"></param>
        /// <returns></returns>
        private bool Overstemming(string suffix)
        {
            string s = TryRemovePart(Term.Length - suffix.Length, suffix.Length);
            if (Vowels.Any(x => s.Contains(x))) {
                return false;
            }
            if (LR.Any(x => s.Contains(x) && !s.EndsWith(x)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// genitive plural for patterns: zena, ulica, gazdina, mesto, srdce ???
        /// </summary>
        private void GenitivePlural()
        {
            string s = new string(Term, 0, TermLength);
            foreach (var item in LongShort)
            {
                if (s.Contains(item.Key))
                {
                    if (LastSyllable(item.Key))
                    {
                        ReplaceLastSyllable(item.Key, item.Value);
                        break;
                    }
                }
            }
        }

        #endregion

        #region Helpers

        private bool StartsWith(string needle)
        {
            return new string(Term, 0, TermLength).StartsWith(needle);
        }

        private bool EndsWith(string needle)
        {
            return new string(Term, 0, TermLength).EndsWith(needle);
        }

        private void AddSuffix(string suffix)
        {
            var builder = new StringBuilder(new string(Term, 0, TermLength));
            builder.Append(suffix);
            Term = builder.ToString().ToCharArray();
            TermLength = Term.Length;
        }

        private void RemovePart(int start, int length)
        {
            var builder = new StringBuilder(new string(Term, 0, TermLength));
            builder.Remove(start, length);
            Term = builder.ToString().ToCharArray();
            TermLength = Term.Length;
        }

        private string TryRemovePart(int start, int length)
        {
            var builder = new StringBuilder(new string(Term, 0, TermLength));
            builder.Remove(start, length);

            return builder.ToString();
        }

        private void ChangeDTNL()
        {
            if (DTNL.Any(x => EndsWith(x.Key)))
            {
                var match = DTNL.First(x => EndsWith(x.Key));
                RemovePart(TermLength - 1, 1);
                AddSuffix(match.Value);
            }
        }

        /// <summary>
        /// check if no vowel is present after needle
        /// </summary>
        /// <param name="needle"></param>
        /// <returns></returns>
        private bool LastSyllable(string needle)
        {
            var term = new string(Term, 0, TermLength);

            int offset = term.LastIndexOf(needle);
            string syllable = term.Substring(offset);
            var lastSyllable = syllable.Substring(needle.Length);

            if (Vowels.Any(x => lastSyllable.Contains(x)))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// replace last occurrence of key with value
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private void ReplaceLastSyllable(string key, string value)
        {
            var term = new string(Term, 0, TermLength);
            int offset = term.LastIndexOf(key);
            string syllable = term.Substring(offset);
            var lastSyllable = syllable.Substring(key.Length);

            Term = (term.Substring(0, offset) + value + lastSyllable).ToCharArray();
            TermLength = Term.Length;
        }

        #endregion

        #region Initialisation

        /// <summary>
        /// Start with longest suffixes, shorter one are the ones contained in longer ones
        /// </summary>
        /// <returns></returns>
        private static List<string[]> CreateSuffixes()
        {
            List<string[]> suffixes = new List<string[]>();     
            suffixes.Add(new string[] {
                "aa", "aom", "at", "atá", "atách", "atám", "atami", "ati", "au", "ä", "äa", "äom", "ät", "ätá", "ätách", "ätám", "ätami", "äti", "äu",
                "ej", "encami", "ence", "encoch", "encom", "eniec", "é", "ého", "ému",
                "iach", "iam", "iami", "ie", "ií", "iou", "iu", "ím",
                "ov", "ovi", "ovia",
                "ú",
                "y", "ý", "ých", "ým", "ými" 
            });
            suffixes.Add(new String[] {
                "ach", "ami", "á", "ách", "ám",
                "e",
                "ia", "ii", "í",
                "o", "och", "om", "ou" });
            suffixes.Add(new String[] { "a", "mi", "u" });
            suffixes.Add(new String[] { "i" });

            return suffixes;
        }

        private static Dictionary<string, string> CreateDTNL()
        {
            Dictionary<string, string> dtnl = new Dictionary<string, string>();
            dtnl.Add("d", "ď");
            dtnl.Add("t", "ť");
            dtnl.Add("n", "ň");
            dtnl.Add("l", "ľ");

            return dtnl;
        }

        private static Dictionary<string, string> CreateLongShort()
        {
            Dictionary<string, string> longShort = new Dictionary<string, string>();
            longShort.Add("á", "a");
            longShort.Add("ie", "e");
            longShort.Add("ĺ", "l");
            longShort.Add("í", "i");
            longShort.Add("ú", "u");
            longShort.Add("ŕ", "r");
            longShort.Add("ô", "o");

            return longShort;
        }

        #endregion
    }
}