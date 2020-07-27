using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
namespace Web.Models
{
    [Table(name:"복습")]
    public class 복습
    {
        [Column(name: "순번"), Key]
        public int 순번 { get; set; }

        [Column(name: "한글발음")]
        public string 한글발음 { get; set; }

        [Column(name: "단어")]
        public string 단어 { get; set; }

        [Column(name: "힌트")]
        public string 힌트 { get; set; }

        [Column(name: "뜻")]
        public string 뜻 { get; set; }

        [Column(name: "암기여부")]
        public string 암기여부 { get; set; }

        [Column(name: "노출횟수")]
        public int 노출횟수 { get; set; }

        [Column(name: "등록일자")]
        public DateTime 등록일자 { get; set; }

        [NotMapped]
        public string 썸네일 { get; set; }
    }
}