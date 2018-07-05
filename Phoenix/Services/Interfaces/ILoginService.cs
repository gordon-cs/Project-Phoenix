using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using Phoenix.Models;

namespace Phoenix.Services
{
    public interface ILoginService
    {
        PrincipalContext ConnectToADServer();
        UserPrincipal FindUser(string username, PrincipalContext ADContext);
        string GenerateToken(string username, string id);
        RoomAssign GetCurrentRoomAssign(string id);
        AccountPermission GetAccountPermissions(string id);
        bool IsValidUser(string username, string password, PrincipalContext ADContext);
        long ToUnixTime(DateTime dateTime);
    }
}