using NUnit.Framework;
using Shouldly;
using LanguageExt;
using static LanguageExt.Prelude;


namespace ConvertMoneyToWords.Tests
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void TestSafeNumber()
        {
            var number = Functions.SafeNumber("1999412.66");
            
            number.ShouldNotBe(0);
            number.ShouldBeGreaterThan(0);
            number.ShouldBe(1999412.66M);
        }

        [Test]
        public void TestSafeNumber_IsZero_OnBadNumber()
        {
            var number = Functions.SafeNumber("bollocks");
            
            number.ShouldBe(0M);
        }

        [Test]
        public void TestSafePounds()
        {
            var example = Some(1999412.66M);
            var result = Functions.SafePounds(example);

            result.ShouldNotBe(0);
            result.ShouldBeGreaterThan(0);
            result.ShouldBe(1999412);
        }

        [TestCase(1999412.66, 66)]
        [TestCase(1999412.667, 67)]
        [TestCase(0.007, 1)]
        [TestCase(0.003, 0)]
        public void TestSafePence(decimal value, int expectedResult)
        {
            var example = Some(value);
            
            var result = Functions.SafePence(example);

            result.ShouldBe(expectedResult);
        }

        [TestCase(1999412.6622451, 1999412.66)]
        [TestCase(1999412, 1999412.00)]
        public void TestFormatCurrency(decimal value, decimal expectedResult)
        {
            var example = Some(value);

            var result = example.FormatCurrency();

            result.ShouldBe(expectedResult);
        }

        [TestCase("1999412.66", 1999412.66)]
        [TestCase("nonsense", 0)]
        public void TestParseDecimal(string value, decimal expectedResult)
        {
            var result = value.ParseDecimal();

            result.ShouldBeOfType<decimal>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(1999412.6622451, 66)]
        [TestCase(0.00, 0)]
        [TestCase(null, 0)]
        public void TestGetPence(decimal value, int expectedResult)
        {
            var example = Some(value);
            var result = example.GetPence();

            result.ShouldBeOfType<int>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(55, "Fifty")]
        [TestCase(77, "Seventy")]
        public void TestGetTens(int value, string expectedResult)
        {
            var result = value.GetTens();

            result.ShouldBeOfType<string>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(55, "Five")]
        [TestCase(77, "Seven")]
        public void TestGetDigits(int value, string expectedResult)
        {
            var result = value.GetDigits();

            result.ShouldBeOfType<string>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(55, 5)]
        [TestCase(77, 7)]
        [TestCase(1999412, 2)]
        public void TestGetLastDigit(int value, int expectedResult)
        {
            var result = value.GetLastDigit();

            result.ShouldBeOfType<int>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(535, 5)]
        [TestCase(737, 7)]
        [TestCase(1999412, 1)]
        public void TestGetFirstDigit(int value, int expectedResult)
        {
            var result = value.GetFirstDigit();

            result.ShouldBeOfType<int>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(30, "Thirty pence")]
        [TestCase(19, "Nineteen pence")]
        [TestCase(1, "One penny")]
        [TestCase(0, "Zero pence")]
        [TestCase(null, "Zero pence")]
        [TestCase(74, "Seventy Four pence")]
        public void TestPenceWords(int value, string expectedResult)
        {
            var example = Some(value);
            var result = example.PenceWords();

            result.ShouldBeOfType<Option<string>>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(1, 1)]
        [TestCase(11, 11)]
        [TestCase(123, 23)]
        [TestCase(12345, 45)]
        public void TestGetTensOption(int value, int expectedResult)
        {
            var example = Some(value);

            var result = example.GetTens();

            result.ShouldBeOfType<Option<int>>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(123, 1)]
        [TestCase(12345, 3)]
        [TestCase(123456, 4)]
        public void TestGetHundredsOption(int value, int expectedResult)
        {
            var example = Some(value);

            var result = example.GetHundreds();

            result.ShouldBeOfType<Option<int>>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(1234, 1)]
        [TestCase(12345, 12)]
        [TestCase(123456, 123)]
        [TestCase(1234567, 234)]
        public void TestGetThousandsOption(int value, int expectedResult)
        {
            var example = Some(value);

            var result = example.GetThousands();

            result.ShouldBeOfType<Option<int>>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(1234123, 1)]
        [TestCase(12345123, 12)]
        [TestCase(123123456, 123)]
        public void TestGetMillionsOption(int value, int expectedResult)
        {
            var example = Some(value);

            var result = example.GetMillions();

            result.ShouldBeOfType<Option<int>>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(55, "Fifty Five pounds")]
        [TestCase(19, "Nineteen pounds")]
        [TestCase(30, "Thirty pounds")]
        [TestCase(0, "Zero pounds")]
        [TestCase(1, "One pound")]
        public void TestGetTensText(int value, string expectedResult)
        {
            var result = value.TensText(value);

            result.ShouldBeOfType<string>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(1, 100, "One hundred")]
        [TestCase(5, 599, "Five hundred")]
        [TestCase(8, 12806, "Eight hundred")]
        [TestCase(0, 0,"")]
        [TestCase(11, 11, "")]
        public void TestHundredsText(int value, int total, string expectedResult)
        {
            var result = value.HundredsText(total);

            result.ShouldBeOfType<string>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(1, "One million")]
        [TestCase(19, "Nineteen million")]
        [TestCase(134, "One hundred and Thirty Four million")]
        [TestCase(0, "")]
        [TestCase(1000, "")]
        public void TestMillionsText(int value, string expectedResult)
        {
            var result = value.MillionsText(Some(value * 1000000));

            result.ShouldBeOfType<string>();
            result.ShouldBe(expectedResult);
        }


        
        [TestCase(55, 155, " and Fifty Five pounds")]
        [TestCase(19, 719, " and Nineteen pounds")]
        [TestCase(30, 1998430, " and Thirty pounds")]
        [TestCase(55, 55, "Fifty Five pounds")]
        [TestCase(19, 19, "Nineteen pounds")]
        [TestCase(30, 30, "Thirty pounds")]
        [TestCase(05, 105, " and Five pounds")]
        [TestCase(5, 5, "Five pounds")]
        [TestCase(0, 0, "Zero pounds")]
        [TestCase(1, 1, "One pound")]
        public void TestGetTensWithAnd(int value, int total, string expectedResult)
        {
            var result = value.TensText(total);
            
            result.ShouldBeOfType<string>();
            result.ShouldBe(expectedResult);
        }

        [TestCase(1, "One pound")]
        [TestCase(155, "One hundred and Fifty Five pounds")]
        [TestCase(0, "Zero pounds")]
        [TestCase(5000, "Five thousand pounds")]
        [TestCase(5555, "Five thousand, Five hundred and Fifty Five pounds")]
        [TestCase(55555, "Fifty Five thousand, Five hundred and Fifty Five pounds")]
        [TestCase(555555, "Five hundred and Fifty Five thousand, Five hundred and Fifty Five pounds")]
        [TestCase(5555555, "Five million, Five hundred and Fifty Five thousand, Five hundred and Fifty Five pounds")]
        [TestCase(55555555, "Fifty Five million, Five hundred and Fifty Five thousand, Five hundred and Fifty Five pounds")]
        [TestCase(555555555, "Five hundred and Fifty Five million, Five hundred and Fifty Five thousand, Five hundred and Fifty Five pounds")]
        [TestCase(5000000, "Five million pounds")]
        [TestCase(5005005, "Five million, Five thousand and Five pounds")]
        [TestCase(505000505, "Five hundred and Five million, Five hundred and Five pounds")]
        public void TestPoundWords(int value, string expectedResult)
        {
            var example = Some(value);
            var result = example.PoundWords();

            result.ShouldBeOfType<Option<string>>();
            result.ShouldBe(expectedResult);
        }



        //
        
        //
    }
}
