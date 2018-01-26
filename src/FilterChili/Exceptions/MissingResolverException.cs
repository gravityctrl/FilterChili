using System;

namespace GravityCTRL.FilterChili.Exceptions
{
    public class MissingResolverException : Exception
    {
        public MissingResolverException(string message) : base(message) {}
    }
}
