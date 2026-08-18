namespace DotNetChallenge.Exceptions
{
    public class DuplicateProductSKUException : ConflictException
    {
        public DuplicateProductSKUException(string message)
            : base(message)
        {
        }
    }
}
