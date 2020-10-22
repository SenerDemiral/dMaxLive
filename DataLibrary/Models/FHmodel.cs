using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLibrary.Models
{
    [Table("FH")]
    public class FHmodel
    {
        [Key]
        public int FhID { get; set; }
        public int DtID { get; set; }
        public int KtID { get; set; }
        public int KhID { get; set; }

        public DateTime? EXD { get; set; }
        public string Skl { get; set; } // Avatar, Document

        [StringLength(40)]
        public string Ad { get; set; }

        [StringLength(10)]
        public string Ext { get; set; }
        public string Path { get; set; }

    }
}
