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

        [TestCase(1, "one pound and zero pence")]
        [TestCase(155, "one hundred and fifty-five pounds and zero pence")]
        [TestCase(0, "zero pounds and zero pence")]
        [TestCase(0.01, "zero pounds and one penny")]
        [TestCase(5000, "five thousand pounds and zero pence")]
        [TestCase(5555.06, "five thousand five hundred and fifty-five pounds and six pence")]
        [TestCase(55555.55, "fifty-five thousand five hundred and fifty-five pounds and fifty-five pence")]
        [TestCase(555555.80, "five hundred and fifty-five thousand five hundred and fifty-five pounds and eighty pence")]
        [TestCase(5555555.99, "five million five hundred and fifty-five thousand five hundred and fifty-five pounds and ninety-nine pence")]
        [TestCase(55555555, "fifty-five million five hundred and fifty-five thousand five hundred and fifty-five pounds and zero pence")]
        [TestCase(555555555.55, "five hundred and fifty-five million five hundred and fifty-five thousand five hundred and fifty-five pounds and fifty-five pence")]
        [TestCase(5000000.01, "five million pounds and one penny")]
        [TestCase(5005005, "five million five thousand and five pounds and zero pence")]
        [TestCase(505000505, "five hundred and five million five hundred and five pounds and zero pence")]
        public void TestPoundWords(decimal value, string expectedResult)
        {
            var example = Some(value);
            var result = example.Map(x => Functions.GetWords(x.ToString()));

            result.ShouldBeOfType<Option<string>>();
            result.ShouldBe(expectedResult);
        }
    }
}
