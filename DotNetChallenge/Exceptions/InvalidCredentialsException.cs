namespace DotNetChallenge.Exceptions
{
    public class InvalidCredentialsException : UnauthorizedException
    {
        public InvalidCredentialsException(string message)
            : base(message)
        {
        }
    }
}
