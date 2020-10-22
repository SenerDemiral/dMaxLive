using System;
using System.Collections.Generic;
using System.Text;

namespace DataLibrary
{
    public static class TableDefs
    {
        public static Dictionary<string, Tbl> TblDefDic = new Dictionary<string, Tbl>();
        public static Dictionary<string, MyTbl> MyTblDefDic = new Dictionary<string, MyTbl>();

        public static void initTableDefs()
        {
            MyTblDefDic.Add("UST", new MyTbl
            {
                PKFieldName = "UstID",
                MyTblFieldDic = new Dictionary<string, MyTblField>()
                {
                    {"UstID", new MyTblField() {Domain = "int32"} },
                    {"Ad",    new MyTblField() {Domain = "string", Length = 40} },
                    {"Usr",   new MyTblField() {Domain = "string", Length = 5} },
                }
            });


            TblDefDic.Add("UST", new Tbl
            {
                PKFieldName = "UstID",
                TblFieldList = new List<TblField>()
                {
                   new TblField() {Name = "UstID", Domain = "int32", PK = true},
                   new TblField() {Name = "Ad",    Domain = "string", Length = 40},
                   new TblField() {Name = "Usr",   Domain = "string", Length = 5},
                   new TblField() {Name = "XXX",   Domain = "string", Length = 5, RO = true}
                }
            });

        }
    }

    public class MyTbl
    {
        public string PKFieldName { get; set; }
        public Dictionary<string, MyTblField> MyTblFieldDic { get; set; }
    }
    public class MyTblField
    {
        public bool RO { get; set; } = false;   // ReadOnly
        public string Domain { get; set; }
        public int Length { get; set; }
    }


    public class Tbl
    {
        public string PKFieldName { get; set; }
        public List<TblField> TblFieldList { get; set; }
    }
    public class TblField
    {
        public bool PK { get; set; } = false;   // PrimaryKey
        public bool RO { get; set; } = false;   // ReadOnly
        public string Name { get; set; }
        public string Domain { get; set; }
        public int Length { get; set; }
        public int MyProperty { get; set; }
    }
}
