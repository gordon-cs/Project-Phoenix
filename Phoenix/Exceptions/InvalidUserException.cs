using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Exceptions
{
    /// <summary>
    /// Exception gets thrown when a user that is not authorized to use the system tries to login. e.g Faculty or non-Admin/non-RD staff
    /// </summary>
    public class InvalidUserException : Exception
    {
        public InvalidUserException()
            : base()
        { }

        public InvalidUserException(string message)
            : base(message)
        { }

        public InvalidUserException(string message, Exception innerException)
            : base(message, innerException)
        { }
    }
}