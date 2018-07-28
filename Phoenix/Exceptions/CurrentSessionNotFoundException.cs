using System;

namespace Phoenix.Exceptions
{
    public class CurrentSessionNotFoundException : Exception 
    {
        public CurrentSessionNotFoundException()
        { }

        public CurrentSessionNotFoundException(string message)
            : base(message) { }

        public CurrentSessionNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}