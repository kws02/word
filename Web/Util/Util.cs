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
    }
}