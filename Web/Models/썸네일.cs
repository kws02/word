using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using System.Web.UI.WebControls;
using System.Windows.Input;

namespace Web.Models
{
    [Table(name: "섹시썸네일")]
    public class 썸네일
    {
        [Column(name:"순번"), Key]
        public int 순번 { get; set; }

        [Column(name: "경로")]
        public string 경로 { get; set; }
    }
}