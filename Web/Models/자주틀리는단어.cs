using Microsoft.ML.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class 자주틀리는단어
    {
        [LoadColumn(0)]
        public int 순번 { get; set; }

        [LoadColumn(1)]
        public string 단어 { get; set; }
    }

    public class 자주틀리는단어Prediction 
    {
        [ColumnName("PredictedLabel")]
        public int 순번;
    }
}