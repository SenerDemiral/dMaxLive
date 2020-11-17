using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DataLibrary.Models;

namespace DataLibrary
{
    public interface IDataAccess
    {
        List<SexModel> SexList();
        List<SklModel> TtSklList();
        List<SklModel> TgSklList();
        Task<T> LoadRec<T, U>(string sql, U parameters);
        Task<List<T>> LoadData<T, U>(string sql, U parameters);
        Task SaveData<T>(string sql, T parameters);
        Task<T> InsertRec<T>(IDictionary<string, object> newValue) where T : new();
        Task UpdateRec<T>(T dataItem, IDictionary<string, object> newValue);

        int GetTablePK(string tblName);
        int GetPK();
        (bool ok, string ad) Login(int usrID, string pwd);
        (bool ok, string ad) Login(MyAppStateModel appState);

        void MyTableFieldsCopy<T, U>(T src, U dst, IDictionary<string, object> newValue);

        Task<MemoryStream> KtCSV(int DtID);
    }
}
