using ConvertMoneyToWords.Enums;
using System;
using System.Globalization;
using LanguageExt;
using static LanguageExt.Prelude;
// ReSharper disable AccessToModifiedClosure


namespace ConvertMoneyToWords
{
    public static class Functions
    {
        public static string GetWords(string value)
        {
            var response = "";
            var workingValue = SafeNumber(value);
            var workingPounds = SafePounds(workingValue);
            var workingPence = SafePence(workingValue);

            var poundWords = "";
            var penceWords = "";

            workingPounds.PoundWords().Match(
                Some: x => poundWords += x,
                None: () => { });

            workingPence.PenceWords().Match(
                Some: x => penceWords += x,
                None: () => { });

            response += poundWords;
            if (!string.IsNullOrEmpty(poundWords) && !string.IsNullOrEmpty(penceWords)) response += " and " + penceWords;

            return response;
        }

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
        {
            var result = 0;
            value.Match(
                x=> result = Convert.ToInt32(x % 1 * 100),
                () => result = 0);
            return result;
        }

        public static decimal GetDecimals(this Option<decimal> value)
        {
            var result = 0M;
            value.Match(
                x=> result = x - Math.Floor(x),
                () => result = 0M);
                
            return result;
        }

        public static string GetTens(this int value)
        { 
            var number = value.GetFirstDigit() * 10;
            return number.ToEnum<BaseNumbers>().ToString();
        }

        public static string GetDigits(this int value)
            => value.GetLastDigit()
                .ToEnum<BaseNumbers>()
                .ToString();

        public static int GetLastDigit(this int value)
            => value % 10;
        
        public static int GetFirstDigit(this int value)
        {
            var result = value;
            while (result >= 10)
            {
                result /= 10;
            }

            return result;
        }

        public static Option<string> PenceWords(this Option<int> value)
        {
            var response = "";
            value.Filter(x => x >= 20 && x % 10 == 0)
                .Map(x => response += x.GetTens());
            value.Filter(x => x >= 20 && x % 10 != 0)
                .Map(x => response += x.GetTens() + " " + x.GetDigits());
            value.Filter(x => x < 20)
                .Map(x => response += x.ToEnum<BaseNumbers>().ToString());
            return response == "One" ? response += " penny" : response += " pence";
        }


        public static Option<int> GetTens(this Option<int> value)
        {
            var stringVal = "";
            value.Match(x => stringVal = x.ToString(),
                () => stringVal = "0");

            var length = 2;
            if (length > stringVal.Length) length = stringVal.Length;
            if(stringVal.Length < 1) return Option<int>.None;

            return parseInt(stringVal
                .AsSpan()
                .Slice(stringVal.Length - length, length)
                .ToString()).IfNone(0);
        }

        
        /// <summary> Gets the last 3 or fewer digits of the integer </summary>
        public static Option<int> GetHundreds(this Option<int> value)
        {
            var stringVal = "";
            value.Match(x => stringVal = x.ToString(),
                () => stringVal = "0");

            var length = 1;
            if (length > stringVal.Length) length = stringVal.Length;
            if(stringVal.Length < 3) return Option<int>.None;

            return parseInt(stringVal
                .AsSpan()
                .Slice(stringVal.Length - 3, length)
                .ToString());
        }

        

        /// <summary> Gets the count of thousands in the integer </summary>
        public static Option<int> GetThousands(this Option<int> value)
        {
            var stringval = "";
            value.Match(x => stringval = x.ToString(),
                () => stringval = "0");

            var length = 1;
            if(stringval.Length > 4) length += stringval.Length - 4;
            if(length > 3) length = 3;
            if(stringval.Length < 4) return Option<int>.None;

            return parseInt(stringval
                .AsSpan()
                .Slice(stringval.Length - (length + 3), length)
                .ToString());
        }

        public static Option<int> GetMillions(this Option<int> value)
        {
            var stringval = "";
            value.Match(x => stringval = x.ToString(),
                () => stringval = "0");
            var length = 1;
            if(stringval.Length > 7) length += stringval.Length - 7;
            if (length > stringval.Length) length = stringval.Length;
            if(stringval.Length < 7) return Option<int>.None;
            if(length > 3) length = 3;

            return parseInt(stringval
                .AsSpan()
                .Slice(stringval.Length - (length + 6), length)
                .ToString());
        }

        public static string HundredsText(this int value, Option<int> total)
        {
            if(value < 1 || value > 9) return string.Empty;
            var response = $"{(BaseNumbers) value.GetFirstDigit()}";
            return response + " hundred";
        }

        public static string ThousandsText(this int value, Option<int> total)
        {
            if(value > 999 || value < 1) return string.Empty;
            var value1 = value * 1000;
            
            var xValue = total.Map(x => (x - value1) / 1000M);
            var hasHundreds = false;
            total.Match(
                Some: x => hasHundreds = Some(x/100M).GetDecimals() > 0.1M,
                None: () => hasHundreds = false);

            var response = "";
            var noAnd = true;
            if (value > 99)
            {
                noAnd = false;
                response += value.GetFirstDigit().HundredsText(value);

                value = Convert.ToInt32(value.ToString().AsSpan().Slice(1, 2).ToString());
                if(value == 0) noAnd = true;
            }

            if(value > 0)
            {
                if(!noAnd) response += " and ";
                var rText = $"{value.TensText(value).Replace(" pounds", "").Replace(" pound", "")}";
                if (!rText.Contains("Zero"))
                {
                    response += rText;
                }
            }
            response += " thousand";
            
            if (xValue.GetDecimals() > 0 && hasHundreds) response += ", ";

            return response;
        }

        public static string MillionsText(this int value, Option<int> total)
        {
            if(value > 999 || value < 1) return string.Empty;
            var value1 = Convert.ToDecimal(value * 1000000M);

            var xValue = total.Map(x => (x - value1) / 1000000M);

            var response = "";
            var noAnd = true;
            if (value > 99)
            {
                noAnd = false;
                response += value.GetFirstDigit().HundredsText(value);

                value = Convert.ToInt32(value.ToString().AsSpan().Slice(1, 2).ToString());
                if(value == 0) noAnd = true;
            }

            if(value > 0)
            {
                if(!noAnd) response += " and ";
                response += $"{value.TensText(value).Replace(" pounds", "").Replace(" pound", "")}";
            }
            response += " million";

            if (xValue.GetDecimals() > 0) response += ", ";

            return response;
        }


        public static string TensText(this int value, Option<int> total)
        {                  
            var resultString = "";
            if(value <= 20 || value.GetLastDigit() == 0) resultString = $"{value.ToEnum<BaseNumbers>().ToString()}";

            if(value.GetDigits() == "Zero") resultString = $"{value.ToEnum<BaseNumbers>().ToString()}";
            
            if(value > 20 && value.GetLastDigit() != 0) resultString = $"{value.GetTens()} {value.GetDigits()}";

            if (value == 0 && total > 99) resultString = "";

            if(total > 99 && value != 0)
            {
                resultString = " and " + resultString;
            }

            resultString += total == 1 ? " pound" : " pounds";

            return resultString;
        }


        public static Option<string> PoundWords(this Option<int> value)
        {
            var response = "";
            value.GetMillions().IfSome(x => response += $"{x.MillionsText(value)}");
            value.GetThousands().IfSome(x => response += $"{x.ThousandsText(value)}");
            value.GetHundreds().IfSome(x => response += $"{x.HundredsText(value)}");
            value.GetTens().IfSome(x => response += $"{x.TensText(value)}");

            return response;
        }
    }
}
