

namespace Application.Exceptions
{
    public class AssignmentConflictException : Exception
    {
        public AssignmentConflictException() { }
        public AssignmentConflictException(string message) : base(message) { }
    }
}
