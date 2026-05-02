using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.Models;

public class ResearchArea
{
    public int Id { get; set; }

    [Required]
    [MaxLength(180)]
    public string Name { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public List<MentorResearchArea> MentorResearchAreas { get; set; } = [];
}
