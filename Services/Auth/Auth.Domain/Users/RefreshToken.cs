namespace Auth.Domain.Users;

public class RefreshToken
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public required string Token { get; set; }

    public DateTime CreatedOn { get; set; }

    public required string CreatedByIp { get; set; }

    public DateTime ExpiresOn { get; set; }

    public DateTime? RevokedOn { get; set; }

    public string? RevokedByIp { get; set; }
}
