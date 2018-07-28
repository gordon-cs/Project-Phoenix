using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Phoenix.Exceptions
{
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