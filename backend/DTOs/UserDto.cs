using MentorReservation.Api.Models;

namespace MentorReservation.Api.DTOs;

public record UserDto(
    int Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    UserRole Role,
    bool IsActive);
