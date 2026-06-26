namespace InvoiceMakerAPI.DTOs
{
    public class AuthResult
    {
        public bool Succeeded { get; init; }

        public string? Token { get; init; }

        public IEnumerable<string> Errors { get; init; } = [];
    }
}
