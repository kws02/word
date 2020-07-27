using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Util
{
    public class SplitHangulJaMo
    {
        private static char[] cho = { 'ㄱ', 'ㄲ', 'ㄴ', 'ㄷ', 'ㄸ', 'ㄹ', 'ㅁ', 'ㅂ', 'ㅃ', 'ㅅ','ㅆ', 'ㅇ', 'ㅈ', 'ㅉ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };
        private static char[] jung = { 'ㅏ', 'ㅐ', 'ㅑ', 'ㅒ', 'ㅓ', 'ㅔ', 'ㅕ', 'ㅖ', 'ㅗ', 'ㅘ','ㅙ', 'ㅚ', 'ㅛ', 'ㅜ', 'ㅝ', 'ㅞ', 'ㅟ', 'ㅠ', 'ㅡ', 'ㅢ','ㅣ' };
        private static char[] jong = { ' ', 'ㄱ', 'ㄲ', 'ㄳ', 'ㄴ', 'ㄵ', 'ㄶ', 'ㄷ', 'ㄹ', 'ㄺ','ㄻ', 'ㄼ', 'ㄽ', 'ㄾ', 'ㄿ', 'ㅀ', 'ㅁ', 'ㅂ', 'ㅄ', 'ㅅ','ㅆ', 'ㅇ', 'ㅈ', 'ㅊ', 'ㅋ', 'ㅌ', 'ㅍ', 'ㅎ' };

        public static char[] Split(char a)
        {
            int x = (int)(a & 0xFFFF);
            int y, z, x1, x2, x3;
            if (x >= 0xAC00 && x <= 0xD7A3)
            {
                y = x - 0xAC00;
                x1 = y / (21 * 28);
                z = y % (21 * 28);
                x2 = z / 28;
                x3 = z % 28;

                if (x3 == 0)
                    return new char[] { cho[x1], jung[x2] };
                else
                    return new char[] { cho[x1], jung[x2], jong[x3] };
            }
            else
                return new char[] { a };
        }

        public static string Split(String s)
        {
            if (string.IsNullOrEmpty(s))
                return "";

            char c = s[0];
            string result = Split(c)[0].ToString();

            return result;
        }
    }
}