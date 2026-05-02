using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.Models;

public class StudentProfile
{
    public int Id { get; set; }

    public int UserId { get; set; }
    public AppUser User { get; set; } = null!;

    [Required]
    [MaxLength(40)]
    public string EnrollmentNumber { get; set; } = string.Empty;

    public StudyProgram StudyProgram { get; set; }

    public int? YearOfStudy { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
