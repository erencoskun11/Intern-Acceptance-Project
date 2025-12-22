using System;

namespace Application.Exceptions
{
    public class DuplicateCategoryAssignmentException : Exception
    {
        public DuplicateCategoryAssignmentException() { }
        public DuplicateCategoryAssignmentException(string message) : base(message) { }
    }
}
