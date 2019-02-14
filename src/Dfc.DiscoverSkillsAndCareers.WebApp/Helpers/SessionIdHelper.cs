using System;
using HashidsNet;

namespace Dfc.DiscoverSkillsAndCareers.WebApp.Helpers
{
    public static class SessionIdHelper
    {
        private const string Alphabet = "acefghjkmnrstwxyz23456789";

        public static string GenerateSessionId(string salt, DateTime date)
        {
            var hashids = new Hashids(salt, 4, Alphabet);
            int rand = Counter();
            string year = (date.Year - 2018).ToString();
            long digits = Convert.ToInt64($"{year}{date.ToString("MMddHHmmssfff")}{rand}");
            var code = hashids.EncodeLong(digits);
            var decode = Decode(salt, code);
            if (digits.ToString() != decode.ToString())
            {
                throw new Exception("Invalid decode");
            }
            return code;
        }

        public static string GenerateSessionId(string salt) => GenerateSessionId(salt, DateTime.UtcNow);

        public static string Decode(string salt, string code)
        {
            var hashids = new Hashids(salt, 4, Alphabet);
            var decoded = hashids.DecodeLong(code);
            if (decoded.Length > 0)
            {
                decoded[0].ToString();
            }
            return null;
        }

        public static string GetYearMonth(string datetimeStamp)
        {
            int yearDigit;
            if (int.TryParse(datetimeStamp.Substring(0, 1), out yearDigit))
            {
                int year = yearDigit + 2018;
                int month;
                if (int.TryParse(datetimeStamp.Substring(1, 2), out month))
                {
                    return new DateTime(year, month, 1).ToString("yyyyMM");
                }
            }
            return null;
        }

        private static int _counter = 10;
        private static readonly object _syncLock = new object();
        public static int Counter()
        {
            lock (_syncLock)
            {
                if (_counter >= 99) _counter = 0;
                _counter++;
                return _counter;
            }
        }
    }
}
