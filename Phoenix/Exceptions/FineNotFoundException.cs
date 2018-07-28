using System;

namespace Phoenix.Exceptions
{
    public class FineNotFoundException : Exception
    {
        public FineNotFoundException()
        { }

        public FineNotFoundException(string message)
            : base(message) { }

        public FineNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}