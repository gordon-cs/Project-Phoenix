using System;

namespace Phoenix.Exceptions
{
    public class DamageNotFoundException : Exception
    {
        public DamageNotFoundException()
        { }

        public DamageNotFoundException(string message)
            : base(message) { }

        public DamageNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}