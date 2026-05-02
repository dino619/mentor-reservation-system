using MentorReservation.Api.Models;
using System.ComponentModel.DataAnnotations;

namespace MentorReservation.Api.DTOs;

public class RegisterStudentDto
{
    [Required]
    [MaxLength(80)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    [MaxLength(160)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MinLength(6)]
    [MaxLength(120)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(40)]
    public string EnrollmentNumber { get; set; } = string.Empty;

    [Required]
    public StudyProgram StudyProgram { get; set; }

    public int? YearOfStudy { get; set; }
}
