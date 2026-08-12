namespace DotNetChallenge.DTOs.Units
{
    public class UnitResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
