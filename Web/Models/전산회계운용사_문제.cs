using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.Models
{
    [Table(name: "전산회계운용사_문제")]
    public class 전산회계운용사_문제
    {
        [Column(name: "년도", Order = 1), Key]
        public string 년도 { get; set; }

        [Column(name: "회차", Order = 2), Key]
        public string 회차 { get; set; }

        [Column(name: "문제번호", Order = 3), Key]
        public int 문제번호 { get; set; }

        [Column(name: "문제")]
        public string 문제 { get; set; }

        [Column(name: "문제유형")]
        public string 문제유형 { get; set; }
    }
}