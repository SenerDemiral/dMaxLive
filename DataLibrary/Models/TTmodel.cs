using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLibrary.Models
{
    [Table("TT")]
    public class TTmodel
    {
        [Key]
        public int TtID { get; set; }
        public int DtID { get; set; }
        public string Ad { get; set; }
        public string Sablon { get; set; }
    }
}
