using System;

namespace Phoenix.Tests.TestUtilities
{
    // Some specially named exceptions to throw while testing.

    /// <summary>
    /// For exceptions that are caused by being in a state we didn't expect to be in. For exmaple, being on 
    /// the dashboard page when we expected to be on the login page.
    /// </summary>
    public class IllegalStateException : Exception
    {
        public IllegalStateException()
        {}

        public IllegalStateException(string message)
            :base(message)
        {}

        public IllegalStateException(string message, Exception inner)
            :base(message, inner)
        {}
    }

    /// <summary>
    /// For exceptions that are caused by calling a function in an unexpected manner. It is up to the test 
    /// writer to make sure they are calling the right functions in the right order and on the right pages.
    /// e.g. If you are on a Rci review page, but you try to call "HitNextToSignatures", which tries to find
    /// and click on the "Next" button. Review pages don't have the next button, so it is up to you to know 
    /// that. 
    /// </summary>
    public class IllegalFunctionCall : Exception
    {
        public IllegalFunctionCall()
        { }

        public IllegalFunctionCall(string message)
            : base(message)
        { }

        public IllegalFunctionCall(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
