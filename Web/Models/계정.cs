using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Web.Models
{
    [Table(name:"계정")]
    public class 계정
    {
        [Column(name:"ID"), Key]
        public string ID { get;set; }

        [Column(name: "PW")]
        public string PW { get; set; }
    }
}