using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Hakon.Core.Extensions;

namespace Hakon.Core.Brain.Utilities
{
    public static class Tokenizer
    {
        public static List<string> GetSentences(string entry){

            entry = entry.Compact();
            var words = GetWords(entry);

            var currentSentence = string.Empty;
            var sentences = new List<string>();

            foreach(var word in words){
                currentSentence = currentSentence + " " + word;

                if(word.IsEndingWord()){
                    sentences.Add(currentSentence.Compact());
                    currentSentence = string.Empty;
                }
            }

            if(currentSentence.IsSet())
                sentences.Add(currentSentence.Compact());

            return sentences;
        }

        public static List<string> GetWords(string entry){
            return entry.Split(" ").ToList();
        }

        private static string Compact(this string entry){
            return entry.Trim().Replace("  ", " ");
        }

        private static bool IsEndingWord(this string word){
            return word.EndsWith(".") || word.EndsWith("!") || word.EndsWith("?");
        }
    }
}