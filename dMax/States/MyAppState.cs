using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataLibrary;
using DataLibrary.Models;

namespace dMax.States
{
    public class MyAppState
    {
        private string _usrName = "";
        private int _usrID = 0;
        readonly IDataAccess _dataAccess;

        public event EventHandler StateChanged;
        private void StateHasChanged()
        {
            // This will update any subscribers
            // that the counter state has changed
            // so they can update themselves
            // and show the current counter value
            StateChanged?.Invoke(this, EventArgs.Empty);
        }
        public MyAppState(IDataAccess dataAccess)
        {
            _dataAccess = dataAccess;
        }

        public bool LoginUser(MyAppStateModel appState)
        {
            //var aaa = _dataAccess.GetPK("UST");

            //var (ok, ad) = DataLibrary.DbAccess.Login(appState.UsrID, appState.UsrPwd);
            var (ok, ad) = _dataAccess.Login(appState.UsrID, appState.UsrPwd);
            if (ok)
            {
                _usrID = appState.UsrID;
                _usrName = ad;
                StateHasChanged();
                return true;
            }
            else if (appState == null)
            {
                _usrID = 0;
                _usrName = "";

                StateHasChanged();
                return false;
            }
            /*
            // Authorize user
            if (appState.UsrPwd == appState.UsrID.ToString())
            {
                _usrID = appState.UsrID;
                _usrName = $"{appState.UsrID}Authorized";

                return true;
            }*/
            return false;
        }
        public void LogoutUser()
        {
            // use usrID LOG vs
            _usrID = 0;
            _usrName = "";
            StateHasChanged();

        }
        public int getUsrID()
        {
            return _usrID;
        }
        public string getUsrName()
        {
            return _usrName;
        }
    }
}
