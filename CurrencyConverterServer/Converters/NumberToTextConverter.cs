using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CurrencyConverterServer.Converters
{
    public class NumberToTextConverter : ICurrencyConverter
    {
        public string Convert(string input)
        {
            var (dollars, cents) = EnsureValidInput(input);

            var dollarsPart = GetDollars(dollars);
            var centsPart = GetCents(cents);

            return string.IsNullOrEmpty(cents)
                ? dollarsPart
                : $"{dollarsPart} and {centsPart}";
        }

        private static (string dollars, string cents) EnsureValidInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentException("Empty input provided.");
            }

            if (input.Any(x => !IsAcceptedChar(x)))
            {
                throw new ArgumentException("Invalid input provided.");
            }

            var noSpacesInput = string.Join("", input.Split(" ", StringSplitOptions.RemoveEmptyEntries));

            var (dollars, cents) = SplitCents(noSpacesInput);

            return (dollars, cents);
        }

        private static bool IsAcceptedChar(char x) => char.IsDigit(x) || char.IsWhiteSpace(x) || x == ',';
        
        private static string GetCents(string cents) => cents switch
        {
            "" => string.Empty,
            "01" => "one cent",
            _ => TranslateCents(cents)
        };

        private static string GetDollars(string dollars) => dollars switch
        {
            "0" => "zero dollars",
            "1" => "one dollar",
            _ => TranslateDollars(dollars)
        };

        private static string TranslateCents(string cents)
        {
            var digit = cents.Length == 2 ? cents[1] : '0';
            var result = GetTwoDigitsNumber(cents[0], digit);
            return $"{result} cents";
        }
  
        private static string TranslateDollars(string dollars)
        {
            var dollarsStack = new Stack<char>(dollars.ToCharArray());
            var quantifiers = new Stack<string>(CurrencyHelper.Quantifiers);
            var result = new Stack<string>();

            while (dollarsStack.Any())
            {
                var quantifier = quantifiers.Pop();
                result.Push(quantifier);

                var digit = dollarsStack.Pop();
                var tens = dollarsStack.Any() ? dollarsStack.Pop() : '0';
                var hundreds = dollarsStack.Any() ? dollarsStack.Pop() : '0';

                var triple = GetThreeDigitsNumber(hundreds, tens, digit);
                result.Push(triple);
            }

            return Print(result);
        }

        private static string ToText(char digit) => ToText($"{digit}");

        private static string ToText(string number) => CurrencyHelper.Translator[number];  

        private static string GetThreeDigitsNumber(char hundreds, char tens, char digit) => (hundreds, tens, digit) switch
        {
            ('0', _, _) => GetTwoDigitsNumber(tens, digit),
            (_, '0', '0') => $"{ToText(hundreds)} hundred",
            _ => $"{ToText(hundreds)} hundred {GetTwoDigitsNumber(tens, digit)}"
        };

        private static string GetTwoDigitsNumber(char tens, char digit) => tens switch
        {
            '0' => ToText(digit),
            '1' => ToText($"{tens}{digit}"),
            _ => $"{ToText($"{tens}0")}-{ToText(digit)}"
        };
        
        private static string Print(Stack<string> numbers)
        {
            var noEmptyEntries = numbers.Where(x => !string.IsNullOrEmpty(x));
            return string.Join(" ", noEmptyEntries);
        }

        private static (string dollars, string cents) SplitCents(string input)
        {
            var segments = input.Split(",");

            var (dollars, cents) = segments.Length switch
            {
                0 => throw new ArgumentException("Empty input provided."),
                1 => (RemoveLeadingZeros(segments[0]), string.Empty),
                2 => (RemoveLeadingZeros(segments[0]), segments[1].TrimEnd('0')),
                _ => throw new ArgumentException("Input has more than one comma.")
            };

            if (cents.Length > 2)
            {
                throw new ArgumentException("Less than 1 cent values are not supported.");
            }

            if (dollars.Length > 9)
            {
                throw new ArgumentException("The maximum number of dollars is 999 999 999.");
            }

            return (dollars, cents);
        }

        private static string RemoveLeadingZeros(string dollars)
        {
            if (dollars.StartsWith('0'))
            {
                var leadingZerosRegex = new Regex(@"^0*");
                var zerosReplacement = dollars.EndsWith('0') ? "0" : string.Empty;
                dollars = leadingZerosRegex.Replace(dollars, zerosReplacement);
            }

            return dollars;
        }
    }
}
