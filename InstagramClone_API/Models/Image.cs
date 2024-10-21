public class Image
{
    public int Id { get; set; }
    public int UserId { get; set; } 
    public string FilePath { get; set; } 
    public string Caption { get; set; } 
    public DateTime CreatedAt { get; set; } = DateTime.Now; 
}
