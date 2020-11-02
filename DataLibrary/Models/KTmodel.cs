using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DataLibrary.Models
{
    [Table("KT")]
    public class KTmodel
    {
        [Key]
        public int KtID { get; set; }
        public int DtID { get; set; }
        public DateTime? EXD { get; set; }
        [Required]
        [StringLength(40)]
        public string Ad { get; set; }

        [Required]
        public DateTime? DgmTrh { get; set; }

        [Required]
        [StringLength(1)]
        public string Sex { get; set; }

        [StringLength(40)]
        public string Mail { get; set; }

        [StringLength(20)]
        public string Tel { get; set; }

        [StringLength(20)]
        public string Pwd { get; set; }

        public int MailOK { get; set; } = -1; // eMail alirim
        public int SmsOK { get; set; } = -1;  // SMS alirim

        public int BoyCm { get; set; }
        public float KiloKg { get; set; }

        public string Info { get; set; }
        public string TGs { get; set; }

        [NotMapped]
        public string _Sex => Sex == "E" ? "Erkek" : "Kadın";

        private IEnumerable<string> _SelectedTGs;
        [NotMapped]
        public IEnumerable<string> SelectedTGs { 
            get
            {
                _SelectedTGs = TGs?.Split(',').ToList();
                return _SelectedTGs;
            }
            set 
            {
                if (value == null)
                    TGs = null;
                else
                    TGs = string.Join(',', value);
                _SelectedTGs = value; 
            }
        }
    }

    public class KTSLmodel  // KT SelectLookup of DT
    {
        public int KtID { get; set; }
        public string Info { get; set; }
    }

}
