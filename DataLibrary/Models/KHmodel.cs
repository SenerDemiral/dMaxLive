using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
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

        // Computed
        public float HedefKg;
        public float IdealKg;
        public float BedenYogunlugu;
        public float BMI;
        public string BMIRad;
        public long Yas;
        public string Ad;
        public string Sex;

        public DateTime? _EXDd => EXD?.Date;
        public string _EXDt => EXD?.ToString("HH:mm");
        public string _BoyCm => $"{BoyCm} cm";
        public string _KiloKg => $"{KiloKg:F1} Kg";
        public string _IdealKg => $"{IdealKg:F1} Kg";
        public string _KasKg => $"{KasKg} Kg";
        public string _YagKg => $"{YagKg} Kg";
        public string _SiviKg => $"{SiviKg} Kg";
        public string _ProteinKg => $"{ProteinKg} Kg";
        public string _MineralKg => $"{MineralKg} Kg";
        public string _BMHkCal => $"{BMHkCal} kCal";
        public string _BedenYogunlugu => $"{BedenYogunlugu:F}";
    }
}
