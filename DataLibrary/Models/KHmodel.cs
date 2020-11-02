using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace DataLibrary.Models
{
    [Table("KH")]
    public class KHmodel
    {
        [Key]
        public int KhID { get; set; }
        public int KtID { get; set; }
        public int DtID { get; set; }
        public DateTime? EXD { get; set; }
        public int BoyCm { get; set; }
        public float KiloKg { get; set; }
        public float KasKg { get; set; }
        public float YagKg { get; set; }
        public float SiviKg { get; set; }
        public string Info { get; set; }
        public float ProteinKg { get; set; }
        public float MineralKg { get; set; }
        public int MetabolikYas { get; set; }
        public float BMHkCal { get; set; }
        public float IcYaglanma { get; set; }
        public string TGs { get; set; }

        // Computed
        public float HedefKg;
        public float IdealKg;
        public float BedenYogunlugu;
        public float BMI;
        public string BMIRad;
        public long Yas;
        public string Ad;
        public DateTime? DgmTrh;
        public string Sex;
        public string Tel;
        public string Mail;

        [NotMapped]
        public DateTime? _EXDd => EXD?.Date;
        [NotMapped]
        public string _EXDt => EXD?.ToString("HH:mm");
        [NotMapped]
        public string _DgmTrh => DgmTrh?.ToString("dd.MM.yyyy");
        [NotMapped]
        public string _TrhZmn => EXD?.ToString("dd.MM.yy HH:mm");
        [NotMapped]
        public string _BoyCm => $"{BoyCm} cm";
        [NotMapped]
        public string _KiloKg => $"{KiloKg:F1} Kg";
        [NotMapped]
        public string _IdealKg => $"{IdealKg:F1} Kg";
        [NotMapped]
        public string _KasKg => $"{KasKg} Kg";
        [NotMapped]
        public string _YagKg => $"{YagKg} Kg";
        [NotMapped]
        public string _SiviKg => $"{SiviKg} Kg";
        [NotMapped]
        public string _ProteinKg => $"{ProteinKg} Kg";
        [NotMapped]
        public string _MineralKg => $"{MineralKg} Kg";
        [NotMapped]
        public string _BMHkCal => $"{BMHkCal} kCal";
        [NotMapped]
        public string _BedenYogunlugu => $"{BedenYogunlugu:F}";

        private IEnumerable<string> _SelectedTGs;
        [NotMapped]
        public IEnumerable<string> SelectedTGs
        {
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
}
