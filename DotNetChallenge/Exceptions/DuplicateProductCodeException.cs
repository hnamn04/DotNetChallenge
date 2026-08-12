namespace DotNetChallenge.Exceptions
{
    public class DuplicateProductCodeException : ConflictException
    {
        public DuplicateProductCodeException(string message)
            : base(message)
        {
        }
    }
}
