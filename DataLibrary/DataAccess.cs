﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
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

            myTableFields = MyTableFields(typeof(KHmodel));
        }

        public List<SexModel> SexList()
        {
            return sexList;
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

        public void MyTableFieldsCopy<T, U>(T src, U dst, IDictionary<string, object> newValue)
        {
            //var ts = typeof(T);
            //var td = typeof(U);

            /*
               dataItem.GetType().GetProperty(n.Key)?.SetValue(dataItem, n.Value);
                }
                var objVal = dataItem.GetType().GetProperty(keyName)?.GetValue(dataItem);
            */
            var aaa = src.GetType().GetProperties();

            foreach (var pd in dst.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty))
            {
                //Console.WriteLine($"{pd.Name} {pd.PropertyType.Name} - {pd.CanWrite}");

                if (pd.CanWrite)
                {
                    var vs = src.GetType().GetProperty(pd.Name)?.GetValue(src);
                    var vd = dst.GetType().GetProperty(pd.Name)?.GetValue(dst);

                    if (!object.Equals(vs, vd))
                    {
                        newValue.Add(pd.Name, vs);
                        dst.GetType().GetProperty(pd.Name)?.SetValue(dst, vs);
                    }
                }
                //dst.GetType().GetProperty(ps.Name).SetValue(td, vs);
            }

        }


    }
}