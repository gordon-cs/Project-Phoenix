using System;

namespace Phoenix.Exceptions
{
    public class RoomAssignNotFoundException : Exception
    {
        public RoomAssignNotFoundException()
        { }

        public RoomAssignNotFoundException(string message)
            : base(message) { }

        public RoomAssignNotFoundException(string message, Exception inner)
            : base(message, inner) { }
    }
}