using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataLibrary.Models
{
    [Table("BMIR")]
    public class BMIRmodel
    {
        [Key]
        public int BmirID { get; set; }
        public int DtID { get; set; }
        public string Sex { get; set; }
        public float Rng { get; set; }
        public string Ad { get; set; }
        public float Nrm { get; set; }  //Normal BMI
        public float Hdf { get; set; }  //Hedef BMI

        public string _Sex => Sex == "E" ? "Erkek" : "Kadın";
    }
}
