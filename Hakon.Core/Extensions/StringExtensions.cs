using System;

namespace Hakon.Core.Extensions
{
    public static class StringExtensions
    {
        public static bool IsSet(this String s){
            return !String.IsNullOrWhiteSpace(s);
        }
    }
}
