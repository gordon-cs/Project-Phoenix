using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Exceptions
{
    /// <summary>
    /// Exception to be thrown by services when a user we expect to find is not found.
    /// </summary>
    public class UserNotFoundException : Exception
    {
        private string user = "";

        public UserNotFoundException(string usr)
            :base(string.Format("The user {0} was not found.", usr))
        {
            user = usr;
        }

        public UserNotFoundException(string usr, Exception innerException)
            : base(string.Format("The user {0} was not found.", usr), innerException)
        {
            user = usr;
        }

        public string NotFoundUser()
        {
            return user;
        }
    }
}