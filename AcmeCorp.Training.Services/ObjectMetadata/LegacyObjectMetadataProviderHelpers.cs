using System;
using System.Linq;

namespace AcmeCorp.Training.Services
{
    internal  static class LegacyObjectMetadataProviderHelpers
    {
        public static Random random = new Random();

        public enum Casing
        {
            Upper,
            Lower,
            Mixed
        }
        public static string GetRandomString(int length, Casing casing, bool withNumbers = true)
        {
            string chars = "";

            switch (casing)
            {
                case Casing.Upper:
                    chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                    break;
                case Casing.Lower:
                    chars = "abcdefghijklmnopqrstuvwxyz";
                    break;
                case Casing.Mixed:
                    chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
                    break;
                default:
                    break;
            }

            if (withNumbers)
            {
                chars += "0123456789";
            }

            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }

        public static string ItemNumber()
        {
            return $"{random.Next(34234)}";
        }

        public static int Version()
        {
            return random.Next(4,9);
        }

        public static string WarehouseId()
        {
            return GetRandomString(7, Casing.Mixed);
        }

        public static string ItemType()
        {
            var types = new string[] { "CoffeGrinder", "ExhaustPipe", "Calculator", "Iron Pipe", "Mobile Toothpick", "Pick", "Guitar pick", "JamJar", "BlendedBeans", "Wine" };
            return types[random.Next(types.Length)];
        }

        public static string ItemGroup()
        {
            return GetRandomString(3, Casing.Upper);
        }

        public static string Market()
        {
            var types = new string[] { "PL", "EN", "DE", "FR", "ZH", "BG", "EL", "SV", "SL" };
            return types[random.Next(types.Length)];
        }

        public static string MarketCode()
        {
            return GetRandomString(3, Casing.Upper);
        }

        public static string Code()
        {
            var str = GetRandomString(5, Casing.Upper);
            return str.Substring(random.Next(3));
        }

        public static string SecondaryCode()
        {
            return Code();
        }


        public static string Guid()
        {
            return System.Guid.NewGuid().ToString();
        }

        public static string Extension()
        {
            var types = new string[] { "xml", "bak.xml", "acm", "xlf" };
            return types[random.Next(types.Length)];
        }
    }
}
