using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.Models
{
    [Table(name:"로그")]
    public class 로그
    { 
        [Column(name: "상세순번", Order = 0), Key]
        public int 상세순번 { get; set; }

        [Column(name: "순번", Order = 1), Key]
        public int 순번 { get; set; }

        [Column(name: "오답여부")]
        public string 오답여부 { get; set; }

        [Column(name: "등록일자")]
        public DateTime 등록일자 { get; set; }
    }
}