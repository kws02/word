using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Web.Models
{
    public class 전산회계운용사
    {
        public string 년도 { get; set; }
        public string 회차 { get; set; }
        public int 문제번호 { get; set; }
        public string 문제 { get; set; }
        public string 문제유형 { get; set; }
        public int 정답번호 { get; set; }
        public int 이미지번호 { get; set; }
        public List<string> 선택 { get; set; }
    }
}