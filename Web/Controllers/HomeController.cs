using Microsoft.ML;
using Microsoft.ML.Transforms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Web.Attribute;
using Web.Models;
using Web.Util;

namespace Web.Controllers
{
    [NoCache]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (Request.Cookies["ID"] != null)
                return RedirectToAction("Main");
            else
                return View();
        }

        [Session]
        public ViewResult Main()
        {
            return View();
        }

        [Session]
        public ViewResult Review()
        {
            return View();
        }

        public int LoginOK(string ID, string PW)
        {
            int Count = 0;

            using (DataContext Context = new DataContext())
            {
                Count = Context.계정.Count(x => x.ID == ID && x.PW == PW);

                if (Count > 0)
                {
                    HttpCookie Cookie = new HttpCookie("ID", ID);
                    Cookie.Expires = DateTime.Now.AddDays(30);
                    Response.Cookies.Add(Cookie);
                }
            }

            return Count;
        }

        [Session]
        public JsonResult ReviewJson(string All, string Search = "")
        {
            List<복습> Review = new List<복습>();

            using (DataContext Context = new DataContext())
            {
                Review = (from t in Context.복습 orderby t.순번 select t).ToList();

                if (All == "N")
                    Review = Review.Where(x => x.암기여부 == "N").ToList();

                if (!string.IsNullOrEmpty(Search))
                    Review = Review.Where(x => x.단어.Contains(Search) || x.뜻.Contains(Search) || x.한글발음.Contains(Search)).ToList();


                foreach (var m in Review)
                {
                    if (string.IsNullOrEmpty(m.한글발음))
                    {
                        m.한글발음 = Utility.WebScript(m.단어);
                        Context.SaveChanges();
                    }

                    if (string.IsNullOrEmpty(m.힌트))
                    {
                        m.힌트 = Utility.StringCut(m.뜻);
                        Context.SaveChanges();
                    }
                }
            }

            return Json(Review, JsonRequestBehavior.AllowGet);
        }

        public string ReviewOK(int Seq, string Flag)
        {
            try
            {
                using (DataContext Context = new DataContext())
                {
                    복습 m = (from n in Context.복습 where n.순번 == Seq select n).SingleOrDefault();

                    if (m != null)
                        m.암기여부 = Flag;

                    Context.SaveChanges();
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }

        [Session]
        public ActionResult Test(string Type = "View")
        {
            List<복습> List = new List<복습>();

            using (DataContext Context = new DataContext())
            {
                using (DbConnection Conn = Context.Database.Connection)
                {
                    Conn.Open();

                    using (DbCommand Cmd = Context.Database.Connection.CreateCommand())
                    {
                        Cmd.CommandText = "dbo.시험보기";
                        Cmd.CommandType = CommandType.StoredProcedure;
                        Cmd.Connection = Conn;

                        using (DbDataReader Rd = Cmd.ExecuteReader())
                        {
                            while (Rd.Read())
                            {

                                List.Add(new 복습()
                                {
                                    순번 = Convert.ToInt32(Rd["순번"]),
                                    한글발음 = Rd["한글발음"].ToString(),
                                    단어 = Rd["단어"].ToString(),
                                    힌트 = Rd["힌트"].ToString(),
                                    뜻 = Rd["뜻"].ToString(),
                                    암기여부 = Rd["암기여부"].ToString(),
                                    노출횟수 = Convert.ToInt32(Rd["노출횟수"]),
                                    썸네일 = ""
                                });
                            }
                        }
                    }

                    ViewBag.남은건수 = Context.복습.Count(x => x.노출횟수 == (Context.복습.Where(y => y.암기여부 == "Y").Min(y => y.노출횟수)));

                    foreach (var m in List)
                    {
                        while (true) 
                        {
                            string 썸네일 = Context.썸네일.OrderBy(y => Guid.NewGuid()).FirstOrDefault().경로;
                            if (!List.Any(x => x.썸네일 == 썸네일))
                            {
                                m.썸네일 = 썸네일;
                                break;
                            }
                         }

                        if (string.IsNullOrEmpty(m.한글발음))
                        {
                            m.한글발음 = Utility.WebScript(m.단어);
                            Context.SaveChanges();
                        }

                        if (string.IsNullOrEmpty(m.힌트))
                        {
                            m.힌트 = Utility.StringCut(m.뜻);
                            Context.SaveChanges();
                        }
                    }
                }
            }

            if (Type == "View")
                return View(List);
            else
                return Json(List, JsonRequestBehavior.AllowGet);
        }

        [Session]
        public ViewResult Library(int Page = 0, int Scroll = 0, string Search = "")
        {
            ViewBag.Page = Page;
            ViewBag.Scroll = Scroll;
            ViewBag.Search = Search;

            return View();
        }

        [Session]
        public ViewResult Insert(int Page = 0, int Seq = 0, int Scroll = 0, string Search = "", string Url = "/Home/Main")
        {
            복습 복습 = new 복습();

            if (Seq > 0)
            {
                using (DataContext Context = new DataContext())
                {
                    복습 = (from m in Context.복습 where m.순번 == Seq select m).SingleOrDefault();
                }
            }

            ViewBag.Page = Page;
            ViewBag.Scroll = Scroll;
            ViewBag.Search = Search;
            ViewBag.Url = Url;

            return View(복습);
        }

        [Session]
        public string Delete(int Seq) 
        {
            try
            {
                using (DataContext Context = new DataContext())
                {
                    Context.Database.ExecuteSqlCommand("DELETE FROM 복습 WHERE 순번 = '" + Seq + "'");
                }
            }
            catch (Exception ex) 
            {
                return ex.Message;
            }

            return "OK";
        }

        public string CheckWord(string Word)
        {
            using (DataContext Context = new DataContext())
            {
                int Count = Context.복습.Count(x => x.단어 == Word);

                if (Count == 0)
                    return "OK";
                else
                    return "DUP";
            }
        }

        public string UpdateOK(int Seq, string Word, string Hangul, string Mean)
        {
            try
            {
                using (DataContext Context = new DataContext())
                {
                    if (Seq > 0)
                    {
                        복습 c = (from m in Context.복습 where m.순번 == Seq select m).SingleOrDefault();
                        c.단어 = Word;
                        c.한글발음 = Hangul;
                        c.뜻 = Mean;
                        c.힌트 = "";

                        Context.SaveChanges();
                    }
                    else
                    {
                        if (CheckWord(Word) == "DUP")
                        {
                            복습 c = (from m in Context.복습 where m.단어 == Word select m).SingleOrDefault();
                            c.단어 = Word;
                            c.한글발음 = Hangul;
                            c.뜻 = Mean;
                            c.힌트 = "";

                            Context.SaveChanges();
                        }
                        else
                        {
                            int NewSeq = Context.복습.Max(x => (int?)x.순번) == null ? 1 : Context.복습.Max(x => x.순번) + 1;
                            Context.Database.ExecuteSqlCommand("INSERT INTO 복습(순번, 단어, 한글발음, 뜻, 등록일자) VALUES ('" + NewSeq + "','" + Word + "','" + Hangul + "','" + Mean + "',GETDATE()+1)");
                        }
                    }
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string TestCountUpdate(int Seq)
        {
            try
            {
                using (DataContext Context = new DataContext())
                {
                    int 노출횟수 = (from m in Context.복습 where m.순번 == Seq select m.노출횟수).SingleOrDefault();

                    복습 c = (from m in Context.복습 where m.순번 == Seq select m).SingleOrDefault();
                    c.노출횟수 = ++노출횟수;

                    Context.SaveChanges();
                    Context.Database.ExecuteSqlCommand("INSERT INTO 로그(순번, 오답여부, 등록일자) VALUES ('" + Seq + "','N', GETDATE())");
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public string WrongCountUpdate(int Seq)
        {
            try
            {
                using (DataContext Context = new DataContext())
                {
                    int 노출횟수 = (from m in Context.복습 where m.순번 == Seq select m.노출횟수).SingleOrDefault();
                    노출횟수--;

                    if (노출횟수 < 1)
                        노출횟수 = 1;

                    복습 c = (from m in Context.복습 where m.순번 == Seq select m).SingleOrDefault();
                    c.노출횟수 = 노출횟수;

                    로그 log = Context.로그.Where(x => x.순번 == Seq).OrderByDescending(x=> x.등록일자).FirstOrDefault();
                    log.오답여부 = "Y";
                    log.등록일자 = DateTime.Now;

                    Context.SaveChanges();
                }

                return "OK";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        [Session]
        public ViewResult Venice()
        {
            return View();
        }

        [Session]
        public ViewResult Option()
        {
            return View();
        }

        public string ClearOK()
        {
            try
            {
                using (DataContext Context = new DataContext())
                {
                    IEnumerable<복습> List = from m in Context.복습 select m;

                    foreach (복습 m in List)
                    {
                        m.노출횟수 = 10;
                    }

                    Context.SaveChanges();
                }

                return "OK";
            }
            catch
            {
                return "ERROR";
            }
        }

        [Session]
        public ViewResult Often()
        {
            List<복습> List = new List<복습>();

            using (DataContext Context = new DataContext())
            {
                List<자주틀리는단어> 틀린내역 = new List<자주틀리는단어>();

                틀린내역.AddRange(from m in Context.복습
                              join n in Context.로그 on m.순번 equals n.순번
                              where n.오답여부 == "Y"
                              && m.암기여부 == "Y"
                              && DbFunctions.AddHours(n.등록일자, 2) >= DateTime.Now
                              group m by new { m.순번, m.단어 } into g
                              select new 자주틀리는단어
                              {
                                  순번 = g.Key.순번,
                                  단어 = g.Key.단어
                              });

                틀린내역.Union(from m in Context.복습
                           join n in Context.로그 on m.순번 equals n.순번
                           where m.암기여부 == "Y"
                           group new { m, n } by new { m.순번, m.단어 } into g
                           where g.Count(x => x.n.오답여부 == "Y") >= g.Count(x => x.n.오답여부 == "N")
                           select new 자주틀리는단어
                           {
                               순번 = g.Key.순번,
                               단어 = g.Key.단어
                           });

                foreach (var m in Context.복습)
                {
                    foreach (var n in 틀린내역)
                    {
                        double per = Utility.CalculateSimilarity(n.단어, m.단어);

                        if (per >= 0.7) 
                            List.Add(m);
                    }
                }

                /*if (틀린내역.Count(x => x.순번 != 0) == 0)
                    return View(List);

                MLContext mlContext = new MLContext();
                IDataView trainingDataView = mlContext.Data.LoadFromEnumerable(틀린내역);

                var pipeline = mlContext.Transforms.Conversion.MapValueToKey(inputColumnName: "순번", outputColumnName: "Label")
                                .Append(mlContext.Transforms.Text.FeaturizeText(inputColumnName: "단어", outputColumnName: "단어Featurized"))
                                .Append(mlContext.Transforms.Concatenate("Features", "단어Featurized"))
                                //.AppendCacheCheckpoint(mlContext)
                                .Append(mlContext.MulticlassClassification.Trainers.SdcaMaximumEntropy(labelColumnName: "Label", featureColumnName: "Features", maximumNumberOfIterations: 20))
                                .Append(mlContext.Transforms.Conversion.MapKeyToValue("PredictedLabel"));

                ITransformer trainedModel = pipeline.Fit(trainingDataView);

                foreach (var m in Context.복습)
                {
                    var data = new 자주틀리는단어() { 단어 = m.단어 };
                    var predition = mlContext.Model.CreatePredictionEngine<자주틀리는단어, 자주틀리는단어Prediction>(trainedModel).Predict(data);

                    if (predition.순번 > 0)
                        List.Add(m);
                }*/
            }

            return View(List);
        }

        public ViewResult Accounting() 
        {
            List<전산회계운용사> 전산회계운용사 = new List<전산회계운용사>();

            using (DataContext dc = new DataContext()) 
            {
                List<전산회계운용사_문제> 재무 = (
                    from m in dc.전산회계운용사_문제
                    where m.문제유형 == "일반" && (m.문제번호 >= 1 && m.문제번호 <= 20)
                    orderby Guid.NewGuid()
                    select m
                ).Take(10).ToList();

                재무.AddRange((
                    from m in dc.전산회계운용사_문제
                    where m.문제유형 == "계산" && (m.문제번호 >= 1 && m.문제번호 <= 20)
                    orderby Guid.NewGuid()
                    select m
                ).Take(10));

                List<전산회계운용사_문제> 원가 = (
                    from m in dc.전산회계운용사_문제
                    where m.문제유형 == "일반" && (m.문제번호 >= 21 && m.문제번호 <= 40)
                    orderby Guid.NewGuid()
                    select m
                ).Take(10).ToList();

                원가.AddRange((
                    from m in dc.전산회계운용사_문제
                    where m.문제유형 == "계산" && (m.문제번호 >= 21 && m.문제번호 <= 40)
                    orderby Guid.NewGuid()
                    select m
                ).Take(10));

                재무 = 재무.OrderBy(x => Guid.NewGuid()).ToList();
                원가 = 원가.OrderBy(x => Guid.NewGuid()).ToList();

                foreach (var m in 재무)
                {
                    전산회계운용사.Add(new 전산회계운용사()
                    {
                        년도 = m.년도,
                        회차 = m.회차,
                        문제번호 = m.문제번호,
                        문제 = m.문제.Substring(m.문제.IndexOf(".")+1).Trim(),
                        문제유형 = m.문제유형,
                        정답번호 = dc.전산회계운용사_답안.SingleOrDefault(x => x.년도 == m.년도 && x.회차 == m.회차 && x.문제번호 == m.문제번호).정답,
                        이미지번호 = dc.전산회계운용사_이미지.SingleOrDefault(x => x.년도 == m.년도 && x.회차 == m.회차 && x.문제번호 == m.문제번호) == null ? 0 : dc.전산회계운용사_이미지.SingleOrDefault(x => x.년도 == m.년도 && x.회차 == m.회차 && x.문제번호 == m.문제번호).이미지번호,
                        선택 = dc.전산회계운용사_선택.Where(x => x.년도 == m.년도 && x.회차 == m.회차 && x.문제번호 == m.문제번호).Select(x => x.선택내용).ToList()
                    });
                }

                foreach (var m in 원가)
                {
                    전산회계운용사.Add(new 전산회계운용사()
                    {
                        년도 = m.년도,
                        회차 = m.회차,
                        문제번호 = m.문제번호,
                        문제 = m.문제.Substring(m.문제.IndexOf(".") + 1).Trim(),
                        문제유형 = m.문제유형,
                        정답번호 = dc.전산회계운용사_답안.SingleOrDefault(x => x.년도 == m.년도 && x.회차 == m.회차 && x.문제번호 == m.문제번호).정답,
                        이미지번호 = dc.전산회계운용사_이미지.SingleOrDefault(x => x.년도 == m.년도 && x.회차 == m.회차 && x.문제번호 == m.문제번호) == null ? 0 : dc.전산회계운용사_이미지.SingleOrDefault(x => x.년도 == m.년도 && x.회차 == m.회차 && x.문제번호 == m.문제번호).이미지번호,
                        선택 = dc.전산회계운용사_선택.Where(x => x.년도 == m.년도 && x.회차 == m.회차 && x.문제번호 == m.문제번호).Select(x => x.선택내용).ToList()
                    });
                }
            }

            return View(전산회계운용사);
        }
    }
}