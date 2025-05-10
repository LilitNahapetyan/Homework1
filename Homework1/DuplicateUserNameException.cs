namespace Homework1
{
    public class DuplicateUserNameException : Exception
    {
        public DuplicateUserNameException(string message) : base(message) { }
    }
}
