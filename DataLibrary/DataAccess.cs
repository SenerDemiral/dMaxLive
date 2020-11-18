using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using DataLibrary.Models;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.Extensions.Configuration;

namespace DataLibrary
{

    public class DataAccess : IDataAccess
    {
        string connectionString = "";

        private readonly IConfiguration _config;
        private Dictionary<string, string> myTableFields = new Dictionary<string, string>();
        private List<SexModel> sexList;
        private List<SklModel> ttSklList;
        private List<SklModel> tgSklList;

        public DataAccess(IConfiguration config)
        {
            _config = config;
            connectionString = config.GetConnectionString("default");
        }
        public DataAccess(string connString)
        {
            connectionString = connString;
            TableDefs.initTableDefs();

            sexList = new List<SexModel>() {
                new SexModel { Sex = "E", Ad = "Erkek" },
                new SexModel { Sex = "K", Ad = "Kadın" },
                new SexModel { Sex = "X", Ad = "XX" }
            };

            ttSklList = new List<SklModel>() {
                new SklModel { Skl = "K", Ad = "Kişi" },
                new SklModel { Skl = "H", Ad = "Hareket" },
                new SklModel { Skl = "M", Ad = "Mail" },
                new SklModel { Skl = "S", Ad = "SMS" }
            };

            tgSklList = new List<SklModel>() {
                new SklModel { Skl = "K", Ad = "Kişi" },
                new SklModel { Skl = "H", Ad = "Hareket" }
            };

            myTableFields = MyTableFields(typeof(KHmodel));
        }

        public List<SexModel> SexList()
        {
            return sexList;
        }

        public List<SklModel> TtSklList()
        {
            return ttSklList;
        }
        public List<SklModel> TgSklList()
        {
            return tgSklList;
        }

        public async Task<T> LoadRec<T, U>(string sql, U parameters)
        {
            using (IDbConnection connection = new FbConnection(connectionString))
            {
                var row = await connection.QueryFirstOrDefaultAsync<T>(sql, parameters);
                return row;
            }
        }


        public async Task<List<T>> LoadData<T, U>(string sql, U parameters)
        {
            using (IDbConnection connection = new FbConnection(connectionString))
            {
                var rows = await connection.QueryAsync<T>(sql, parameters);

                return rows.ToList();
            }
        }

        public Task SaveData<T>(string sql, T parameters)
        {
            using (IDbConnection connection = new FbConnection(connectionString))
            {
                return connection.ExecuteAsync(sql, parameters);
            }
        }

        public async Task UpdateRec<T>(T dataItem, IDictionary<string, object> newValue)
        {
            StringBuilder sql = new System.Text.StringBuilder();
            int dc = newValue.Count;
            bool first = true;
            if (dc > 0)
            {
                Type t = typeof(T);

                var (tblName, keyName) = MyTableNameAndKey(t);


                sql.Append($"update {tblName} set ");
                foreach (var n in newValue)
                {
                    sql.Append($"{(first ? "" : ",")} {n.Key} = @{n.Key} ");
                    first = false;
                    // newValue yu dataItem a da koy, UI de degissin
                    dataItem.GetType().GetProperty(n.Key)?.SetValue(dataItem, n.Value);
                }
                var objVal = dataItem.GetType().GetProperty(keyName)?.GetValue(dataItem);
                sql.Append($" where {keyName} = {objVal}");  // PK
                Console.WriteLine(sql.ToString());
                await SaveData(sql.ToString(), newValue);
            }
        }

        public async Task<T> InsertRec<T>(IDictionary<string, object> newValue) where T : new()
        {
            //string sql = "insert into UST (UstID, Ad) values (@UstID, @Ad)";
            var dataItem = new T();

            int dc = newValue.Count;
            if (dc > 0)
            {
                bool first = true;
                StringBuilder sql = new StringBuilder();
                StringBuilder prm = new StringBuilder();
                var (tblName, keyName) = MyTableNameAndKey(typeof(T));

                newValue.Add(keyName, GetPK()); // Get PK

                sql.Append($"insert into {tblName}(");
                prm.Append("values(");

                foreach (var n in newValue)
                {
                    sql.Append($"{(first ? "" : ",")}{n.Key} ");
                    prm.Append($"{(first ? "" : ",")}@{n.Key} ");
                    first = false;

                    dataItem.GetType().GetProperty(n.Key)?.SetValue(dataItem, n.Value); // Populate new rec
                }
                sql.Append(")");
                prm.Append(")");

                await SaveData($"{sql} {prm}", newValue);
            }

            return dataItem;
        }

        public int GetTablePK(string tblName)
        {
            int pk = 0;
            using (IDbConnection connection = new FbConnection(connectionString))
            {
                pk = connection.ExecuteScalar<int>($"select ID from Get_PK('{tblName}')");
            }
            return pk;
        }
        public int GetPK()
        {
            int pk = 0;
            using (IDbConnection connection = new FbConnection(connectionString))
            {
                pk = connection.ExecuteScalar<int>($"select ID from Get_PK");
            }
            return pk;
        }

        public (bool ok, string ad) Login(int usrID, string pwd)
        {
            string ok = "F", ad = "";
            string sql = "select OK, UsrAd from LOGIN(@UsrID, @Pwd)";
            LoginResultModel res = new LoginResultModel();
            using (IDbConnection connection = new FbConnection(connectionString))
            {
                res = connection.QueryFirst<LoginResultModel>(sql, new { UsrID = usrID, Pwd = pwd });
                //res = connection.QueryFirst<LoginResultModel>(sql, (UsrID: usrID, Pwd: pwd));
            }
            ok = res.OK;
            ad = res.USRAD;
            return (ok == "T", ad);
        }
        public (bool ok, string ad) Login(MyAppStateModel appState)
        {
            string ok = "F", ad = "";
            string sql = "select OK, UsrAd from LOGIN(@UsrID, @Pwd)";
            LoginResultModel res = new LoginResultModel();
            using (IDbConnection connection = new FbConnection(connectionString))
            {
                res = connection.QueryFirst<LoginResultModel>(sql, new { UsrID = appState.UsrID, UsrPwd = appState.UsrPwd });
            }
            ok = res.OK;
            ad = res.USRAD;
            return (ok == "T", ad);
        }

        public (string tblName, string keyName) MyTableNameAndKey(Type t)
        {
            string tblName = "", keyName = "";
            TableAttribute ta = (TableAttribute)t.GetCustomAttribute(typeof(TableAttribute));
            if (ta != null)
                tblName = ta.Name;

            var properties = t.GetProperties();
            foreach (var property in properties)
            {
                var pAttributes = property.GetCustomAttributes(false);
                KeyAttribute ka = (KeyAttribute)property.GetCustomAttribute(typeof(KeyAttribute));
                if (ka != null)
                {
                    keyName = property.Name;
                    break;
                }
            }
            return (tblName, keyName);
        }

        public Dictionary<string, string> MyTableFields(Type t)
        {
            Dictionary<string, string> d = new Dictionary<string, string>();

            //foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            foreach (var prop in t.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                //Console.WriteLine($"{prop.Name} {prop.PropertyType.Name} - {prop.CanWrite}");
                d.Add(prop.Name, prop.PropertyType.Name);
            }

            return d;
        }

        public void MyTableFieldsCopy<T, U>(T edtCtx, U oldRow, IDictionary<string, object> newValue)
        {
            // edtCtx::EditFormContext (oldRow disinda baska alanlar da var)
            // oldRow::??model bunun uzerinden git
            foreach (var fld in oldRow.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                NotMappedAttribute nma = (NotMappedAttribute)fld.GetCustomAttribute(typeof(NotMappedAttribute));

                //if (fld.Name != "SelectedTGs") // SelectedTGs Table da yok: NotMapped olarak isaretlendi
                if (nma == null)
                {
                    if (fld.CanWrite)
                    {
                        var newVal = edtCtx.GetType().GetProperty(fld.Name)?.GetValue(edtCtx);
                        var oldVal = oldRow.GetType().GetProperty(fld.Name)?.GetValue(oldRow);

                        if (!object.Equals(newVal, oldVal))
                        {
                            newValue.Add(fld.Name, newVal);
                            //oldRow.GetType().GetProperty(fld.Name)?.SetValue(oldRow, newVal); // Gerek yok, cagiran yapiyor
                        }
                    }
                }
            }
        }
        
        public async Task<MemoryStream> KtCSV(int DtID)
        {
            string sql = $"select * from KT where DtID = {DtID}";
            List<KTmodel> data = await LoadData<KTmodel, dynamic>(sql, new { });

            StringBuilder sb = new StringBuilder();

            //sb.Append('\uFEFF');    // U+FEFF is the byte-order mark (BOM) character
            sb.Append("ID,Ad,Sex,DgmTrh,Mail,Tel,Info");
            sb.AppendLine();

            foreach (var row in data)
						{
                sb.Append($"{row.KtID}");
                sb.Append(",");
                sb.Append($"\"{row.Ad}\"");
                sb.Append(",");
                sb.Append($"\"{row.Sex}\"");
                sb.Append(",");
                sb.Append($"{row.DgmTrh:dd.MM.yyyy}");
                sb.Append(",");
                sb.Append($"\"{row.Mail}\"");
                sb.Append(",");
                sb.Append($"\"{row.Tel}\"");
                sb.Append(",");
                sb.Append($"\"{row.Info?.Replace("\"", "\"\"")}\"");
                //sb.Append($"\"{row.Info?.Replace("\"", "\"\"")}\"").Replace("\n", "\"\n\"");
                sb.AppendLine();
            }

            //FileStream stream = new FileStream("C:\\Net5.0\\sener.csv", FileMode.Create);
            //using (var writer = new StreamWriter(stream, System.Text.Encoding.UTF8))
            //{
            //    writer.Write(sb.ToString());
            //}

            //using calismiyor
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.UTF8);

            writer.Write(sb.ToString());
            writer.Flush();
            ms.Position = 0;
            return ms;
        }

        public async Task<MemoryStream> KhCSV(int DtID)
        {
            string sql = $"select * from KH where DtID = {DtID}";
            List<KHmodel> data = await LoadData<KHmodel, dynamic>(sql, new { });

            StringBuilder sb = new StringBuilder();

            //sb.Append('\uFEFF');    // U+FEFF is the byte-order mark (BOM) character
            sb.Append("ID,KtID,EXD,Info");
            sb.AppendLine();

            foreach (var row in data)
            {
                sb.Append($"{row.KhID}");
                sb.Append(",");
                sb.Append($"{row.KtID}");
                sb.Append(",");
                sb.Append($"{row.EXD:dd.MM.yyyy}");
                sb.Append(",");
                sb.Append($"\"{row.Info?.Replace("\"", "\"\"")}\"");
                sb.AppendLine();
            }

            var ms = new MemoryStream();
            var writer = new StreamWriter(ms, Encoding.UTF8);

            writer.Write(sb.ToString());
            writer.Flush();
            ms.Position = 0;
            return ms;
        }
        public DTmodel GetDTrec(int DtID)
				{
            using (IDbConnection connection = new FbConnection(connectionString))
            {
                return connection.QueryFirstOrDefault<DTmodel>($"select * from DT where DtID = {DtID}");
            }
        }
    }
}
