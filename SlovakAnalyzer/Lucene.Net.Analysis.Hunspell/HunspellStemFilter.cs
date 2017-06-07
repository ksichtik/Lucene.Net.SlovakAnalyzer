// <copyright file="HunspellStemFilter.cs">
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
// 
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using Lucene.Net.Analysis.Tokenattributes;

namespace Lucene.Net.Analysis.Hunspell {
    /// <summary>
    ///   TokenFilter that uses hunspell affix rules and words to stem tokens.  Since hunspell supports a
    ///   word having multiple stems, this filter can emit multiple tokens for each consumed token.
    /// </summary>
    public class HunspellStemFilter : TokenFilter {
        private readonly TermAttribute _termAtt;
        private readonly PositionIncrementAttribute _posIncAtt;
        private readonly HunspellStemmer _stemmer;
        private readonly SlovakStemmer _slovakStemmer;

        private readonly Queue<HunspellStem> _buffer = new Queue<HunspellStem>();
        private State _savedState;

        private readonly Boolean _dedup;

        /// <summary>
        ///   Creates a new HunspellStemFilter that will stem tokens from the given TokenStream using
        ///   affix rules in the provided HunspellDictionary.
        /// </summary>
        /// <param name="input">TokenStream whose tokens will be stemmed.</param>
        /// <param name="dictionary">HunspellDictionary containing the affix rules and words that will be used to stem the tokens.</param>
        /// <param name="dedup">true if only unique terms should be output.</param>
        public HunspellStemFilter(TokenStream input, HunspellDictionary dictionary, Boolean dedup = true)
            : base(input) {
            _posIncAtt = (PositionIncrementAttribute)AddAttribute(typeof(PositionIncrementAttribute));
            _termAtt = (TermAttribute)AddAttribute(typeof(TermAttribute));

            _dedup = dedup;
            _stemmer = new HunspellStemmer(dictionary);
            _slovakStemmer = new SlovakStemmer();
        }

        public override Boolean IncrementToken() {
            if (_buffer.Any()) {
                var nextStem = _buffer.Dequeue();

                RestoreState(_savedState);
                _posIncAtt.SetPositionIncrement(0);
                _termAtt.SetTermBuffer(nextStem.Stem, 0, nextStem.StemLength);
                return true;
            }

            if (!input.IncrementToken())
                return false;

            var newTerms = _dedup
                               ? _stemmer.UniqueStems(_termAtt.Term())
                               : _stemmer.Stem(_termAtt.Term());
            foreach (var newTerm in newTerms)
                _buffer.Enqueue(newTerm);

            if (_buffer.Count == 0)
            {
                // originaly: we do not know this word, return it unchanged
                // changed: apply SlovakStemmer on words not found in dictionary (possible named entities)
                var currentTerm = new string(_termAtt.TermBuffer(), 0, _termAtt.TermLength());
                if (!string.IsNullOrEmpty(currentTerm))
                {
                    _slovakStemmer.Stem(_termAtt.TermBuffer(), _termAtt.TermLength(), out char[] newTerm, out var newLength);
                    _termAtt.SetTermBuffer(newTerm, 0, newLength);
                    _termAtt.SetTermLength(newLength);
                }
                return true;
            }  

            var stem = _buffer.Dequeue();
            _termAtt.SetTermBuffer(stem.Stem, 0, stem.StemLength);

            if (_buffer.Count > 0)
                _savedState = CaptureState();

            return true;
        }

        public override void Reset() {
            base.Reset();

            _buffer.Clear();
        }
    }
}
