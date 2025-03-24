using System.ComponentModel.DataAnnotations;

public class Review
{
    public int Id { get; set; }
    public int ProductId { get; set; }

    public string? UserName { get; set; } // ❌ No [Required], set in controller

    [Required]
    public int Rating { get; set; }

    [Required]
    public string? Comment { get; set; }

    public DateTime Date { get; set; } = DateTime.Now;
}
    