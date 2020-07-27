using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Web.Util
{
    public class Utility
    {
        public static string WebScript(string Word)
        {
            string Result = string.Empty;

            WebRequest Request = WebRequest.Create("http://aha-dic.com/View.asp?word=" + Word);
            Request.Method = "GET";
            WebResponse Response = Request.GetResponse();

            using (StreamReader Sr = new StreamReader(Response.GetResponseStream()))
            {
                Result = Sr.ReadToEnd();

                if (Result.IndexOf("<title></title>") == -1)
                {
                    Result = Result.Substring(Result.IndexOf("<title>"));
                    Result = Result.Substring(0, Result.IndexOf("</title>"));
                    Result = Result.Substring(Result.LastIndexOf("[") + 1);
                    Result = Result.Substring(0, Result.LastIndexOf("]"));
                }
                else
                    Result = "";
            }

            return Result;
        }

        public static string StringCut(string Word)
        {
            string Result = string.Empty;

            if (Word.Length > 1)
                Result = Word.Substring(0, 1);
            else if (Word.Length == 1)
                Result = SplitHangulJaMo.Split(Word.Substring(0, 1));

            return Result;
        }

        private static int ComputeLevenshteinDistance(string source, string target)
        {
            if ((source == null) || (target == null)) return 0;
            if ((source.Length == 0) || (target.Length == 0)) return 0;
            if (source == target) return source.Length;

            int sourceWordCount = source.Length;
            int targetWordCount = target.Length;

            // Step 1
            if (sourceWordCount == 0)
                return targetWordCount;
            
            if (targetWordCount == 0)
                return sourceWordCount;

            int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

            // Step 2
            for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++);
            for (int j = 0; j <= targetWordCount; distance[0, j] = j++);

            for (int i = 1; i <= sourceWordCount; i++)
            {
                for (int j = 1; j <= targetWordCount; j++)
                {
                    // Step 3
                    int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                    // Step 4
                    distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
                }
            }

            return distance[sourceWordCount, targetWordCount];
        }

        public static double CalculateSimilarity(string source, string target)
        {
            if ((source == null) || (target == null)) return 0.0;
            if ((source.Length == 0) || (target.Length == 0)) return 0.0;
            if (source == target) return 1.0;

            int stepsToSame = ComputeLevenshteinDistance(source, target);

            return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
        }
    }
}