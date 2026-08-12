namespace DotNetChallenge.Common.Helpers
{
    public static class StringHelper
    {
        public static string? Normalize(string? value)
        {
            return string.IsNullOrWhiteSpace(value)
                ? null
                : value.Trim();
        }

        public static string? NormalizePhone(string? phone)
        {
            return string.IsNullOrWhiteSpace(phone)
                ? null
                : phone.Trim();
        }
    }
}
