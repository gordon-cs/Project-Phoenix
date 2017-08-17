using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Exceptions
{
    /// <summary>
    /// Exception thrown by services when we expect to find a record in the database, but don't find it. 
    /// </summary>
    public class DatabaseEntryNotFound : Exception
    {
        public DatabaseEntryNotFound()
            : base()
        { }

        public DatabaseEntryNotFound(string message)
            : base(message)
        { }

        public DatabaseEntryNotFound(string message, Exception innerException)
            :base(message, innerException)
        { }
    }
}