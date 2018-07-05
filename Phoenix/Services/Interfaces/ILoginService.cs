using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using Phoenix.DapperDal.Types;

namespace Phoenix.Services
{
    public interface ILoginService
    {
        PrincipalContext ConnectToADServer();
        UserPrincipal FindUser(string username, PrincipalContext ADContext);
        string GenerateToken(string username, string id);
        RoomAssignment GetCurrentRoomAssign(string id);
        AccountPermission GetAccountPermissions(Account account);
        bool IsValidUser(string username, string password, PrincipalContext ADContext);
        long ToUnixTime(DateTime dateTime);
    }
}