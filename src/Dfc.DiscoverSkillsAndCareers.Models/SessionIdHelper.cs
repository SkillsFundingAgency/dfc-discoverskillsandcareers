using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using HashidsNet;

namespace Dfc.DiscoverSkillsAndCareers.Models
{
    public static class CryptoRandom
    {
        private static System.Security.Cryptography.RNGCryptoServiceProvider rnd = new System.Security.Cryptography.RNGCryptoServiceProvider();

        public static int Next(int i)
        {
            byte[] r = new byte[4];
            int value;

            do
            {
                rnd.GetBytes(r);
                value = BitConverter.ToInt32(r, 0) & Int32.MaxValue;
            } while (value >= i * (Int32.MaxValue / i));

            return value % i;
        }
    }

    public static class ArrayExtensions
    {
        public static T[] FisherYatesShuffle<T>(this IEnumerable<T> source)
        {
            int n = source.Count();
            var target = new T[n];

            Array.Copy(source.ToArray(), target, n);

            while (n > 1)
            {
                n--;
                int k = CryptoRandom.Next(n + 1);
                T value = target[k];
                target[k] = target[n];
                target[n] = value;
            }

            return target;
        }
    }


    public class ReloadCodeGenerator
    {
        private readonly string[] words;
        private readonly int wordCount;
        
        public int DictionarySize => words.Length;
        
        public ReloadCodeGenerator(string[] words, int wordCount = 3)
        {
            this.words = words.FisherYatesShuffle();
            this.wordCount = wordCount;
        }

        public ReloadCodeGenerator(Stream fileStream, int wordCount = 3)
        {
            var list = new List<string>();
            using (var sr = new StreamReader(fileStream))
            {
                var line = sr.ReadLine();
                while (!sr.EndOfStream)
                {
                    list.Add(line);
                    line = sr.ReadLine();
                }
            }

            this.words = list.ToArray();
            this.wordCount = wordCount;
        }

        public string Generate()
        {
            var dictionary = this.words;
            var sb = new StringBuilder();

            for (var i = 0; i < this.wordCount; i++)
            {
                sb.Append(dictionary[CryptoRandom.Next(dictionary.Length)]);
                sb.Append(" ");
            }

            return sb.ToString().Trim();
        }

    }

    public static class SessionIdHelper
    {
        //private const string Alphabet = "acefghjkmnrstwxyz23456789";

        //public static string GenerateSessionId(string salt, DateTime date)
        //{
        //    var hashids = new Hashids(salt, 4, Alphabet);
        //    int rand = Counter();
        //    string year = (date.Year - 2018).ToString();
        //    long digits = Convert.ToInt64($"{year}{date.ToString("MMddHHmmssfff")}{rand}");
        //    var code = hashids.EncodeLong(digits);
        //    var decode = Decode(salt, code);
        //    if (digits.ToString() != decode.ToString())
        //    {
        //        throw new Exception("Invalid decode");
        //    }
        //    return code;
        //}
        
        private static ReloadCodeGenerator reloadCodeGenerator;

        public static void Initialize(string wordDictionaryPath)
        {
            using (var fs = File.OpenRead(wordDictionaryPath))
            {
                reloadCodeGenerator = new ReloadCodeGenerator(fs);
            }
        }
        
        public static string GenerateSessionId() => Guid.NewGuid().ToString();

        public static string GenerateReloadCode() => reloadCodeGenerator.Generate();

        //public static string Decode(string salt, string code)
        //{
        //    var hashids = new Hashids(salt, 4, Alphabet);
        //    var decode = hashids.DecodeLong(code);
        //    if (decode.Length > 0)
        //    {
        //        return decode[0].ToString();
        //    }
        //    return null;
        //}

        //public static string GetYearMonth(string datetimeStamp)
        //{
        //    int yearDigit;
        //    if (int.TryParse(datetimeStamp.Substring(0, 1), out yearDigit))
        //    {
        //        int year = yearDigit + 2018;
        //        int month;
        //        if (int.TryParse(datetimeStamp.Substring(1, 2), out month))
        //        {
        //            return new DateTime(year, month, 1).ToString("yyyyMM");
        //        }
        //    }
        //    return null;
        //}

        //private static int _counter = 10;
        //private static readonly object _syncLock = new object();
        //public static int Counter()
        //{
        //    lock (_syncLock)
        //    {
        //        if (_counter >= 99) _counter = 0;
        //        _counter++;
        //        return _counter;
        //    }
        //}
    }
}
