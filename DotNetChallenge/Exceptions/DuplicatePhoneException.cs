namespace DotNetChallenge.Exceptions
{
    public class DuplicatePhoneException : ConflictException
    {
        public DuplicatePhoneException(string message)
            : base(message)
        {
        }
    }
}
