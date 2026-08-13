namespace DotNetChallenge.Exceptions
{
    public class DuplicateEmailException : ConflictException
    {
        public DuplicateEmailException(string message)
            : base(message)
        {
        }
    }
}
