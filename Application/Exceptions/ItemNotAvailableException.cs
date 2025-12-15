using System;

namespace Application.Exceptions
{
    public class ItemNotAvailableException : Exception
    {
        public ItemNotAvailableException() { }
        public ItemNotAvailableException(string message) : base(message) { }
    }
}
