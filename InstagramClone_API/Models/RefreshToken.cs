public class RefreshToken
{
    public int Id { get; set; }
    public int UserId { get; set; } 
    public string Token { get; set; } 
    public DateTime Expires { get; set; }
    public bool IsExpired => DateTime.UtcNow >= Expires; 
    public DateTime Created { get; set; } = DateTime.UtcNow;
    public DateTime? Revoked { get; set; } 
    public bool IsRevoked => Revoked != null; 
    public bool IsActive => !IsExpired && !IsRevoked; 
}
