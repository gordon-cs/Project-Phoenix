using System;

namespace Phoenix.Exceptions
{
    public class RciNotFoundException : Exception
    {
        public RciNotFoundException()
        { }

        public RciNotFoundException(string message)
            : base(message) { }

        public RciNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}