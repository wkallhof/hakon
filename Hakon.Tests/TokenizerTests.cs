using System.Linq;
using Hakon.Core.Brain.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hakon.Tests
{
    [TestClass]
    public class TokenizerTests
    {
        private TestHarness _t;

        [TestInitialize]
        public void Init(){
            this._t = new TestHarness();
        }

        [TestMethod]
        public void Get_sentences_should_return_correct_sentences(){
            var result = Tokenizer.GetSentences(this._t.TestSentenceEntry);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual(this._t.ValidSentenceOne, result.ElementAt(0));
            Assert.AreEqual(this._t.ValidSentenceTwo, result.ElementAt(1));
            Assert.AreEqual(this._t.ValidSentenceThree, result.ElementAt(2));
            Assert.AreEqual(this._t.ValidSentenceFour, result.ElementAt(3));
        }

        [TestMethod]
        public void Get_sentences_should_handle_a_single_sentence(){
            var result = Tokenizer.GetSentences("Hello.");

            Assert.AreEqual(1, result.Count);
        }

        [TestMethod]
        public void Get_sentences_should_handle_an_empty_sentence(){
            var result = Tokenizer.GetSentences("");

            Assert.AreEqual(0, result.Count);
        }

        [TestMethod]
        public void Get_sentences_should_handle_no_ending(){
            var result = Tokenizer.GetSentences("Bon sang ce n'est pas ça. Bon sang");

            Assert.AreEqual(2, result.Count);
        }

        [TestMethod]
        public void Get_words_returns_correct_words(){
            var result = Tokenizer.GetWords("Très bien Hakon.");

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Très", result.ElementAt(0));
            Assert.AreEqual("bien", result.ElementAt(1));
            Assert.AreEqual("Hakon.", result.ElementAt(2));
        }



        public class TestHarness{
            public string ValidSentenceOne = "Nous allons bien voir ce que ça donne!";
            public string ValidSentenceTwo = "N'est-ce pas ?";
            public string ValidSentenceThree = "Et avec une URL en plus, c'est mieux: http://google.com.";
            public string ValidSentenceFour = "Mais il nous manque encore un mail: gg@gggg.kk";

            public string TestSentenceEntry = "Nous allons bien voir ce que ça   donne!" +
                    " N'est-ce pas ? " +
                    " Et avec une URL en plus, c'est mieux: http://google.com." +
                    " Mais il nous manque encore un mail: gg@gggg.kk";
        }
    }
}