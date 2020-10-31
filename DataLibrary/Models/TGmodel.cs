using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLibrary.Models
{
    [Table("TG")]
    public class TGmodel
    {
        [Key]
        public int TgID { get; set; }
        public int DtID { get; set; }
        public string Skl { get; set; }
        public string Ad { get; set; }
        public string IDS { get; set; }
    }
}
