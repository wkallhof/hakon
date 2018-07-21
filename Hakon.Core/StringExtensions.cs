using System;

namespace Hakon.Core
{
    public static class StringExtensions
    {
        public static bool IsSet(this String s){
            return !String.IsNullOrWhiteSpace(s);
        }
    }
}
