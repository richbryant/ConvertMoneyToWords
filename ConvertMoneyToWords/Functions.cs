using System;
using System.Globalization;
using Humanizer;
using LanguageExt;
using static LanguageExt.Prelude;


namespace ConvertMoneyToWords
{
    public static class Functions
    {
        public static string GetWords(string number)
        {
            var value = SafeNumber(number);

            if (value >= 1000000000 || value < 0)
            {
                return "Invalid entry";
            }

            var workingPounds = SafePounds(value);
            var workingPence = SafePence(value);

            var pounds = workingPounds.Bind(x => FormatPluralPounds(x));
            var pence = workingPence.Bind(x => FormatPluralPence(x));

            return pounds.Match(x => x, () => "") + " and " + pence.Match(x => x, () => "");
        }

        private static Option<string> FormatPluralPounds(Option<int> number)
            => number.IsSome && number == 1
                    ? number.AsWords().Map(x => x + " pound")
                    : number.AsWords().Map(x => x + " pounds");

        private static Option<string> FormatPluralPence(Option<int> number)
            => number.IsSome && number == 1
                ? number.AsWords().Map(x => x + " penny")
                : number.AsWords().Map(x => x + " pence");
        

        public static Option<decimal> SafeNumber(string value)
            => parseDecimal(value).IfNone(0);
        
        public static Option<int> SafePounds(Option<decimal> value)
            => value.Bind(x => parseInt(Math.Truncate(x).ToString(CultureInfo.InvariantCulture)));
         
        public static Option<int> SafePence(Option<decimal> value)
            => value.Map(x => value.FormatCurrency()
                                   .GetPence());
     
        public static Option<decimal> FormatCurrency(this Option<decimal> value)
            => value.Map(x => $"{x:F2}".ParseDecimal());

        public static decimal ParseDecimal(this string value)
            => parseDecimal(value).IfNone(0);

        public static int GetPence(this Option<decimal> value)
            => value.Match(x=> Convert.ToInt32(x % 1 * 100),
                () => 0);
        
        public static Option<string> AsWords(this Option<int> value)
            => value.Map(x => x.ToWords());
        
    }
}
