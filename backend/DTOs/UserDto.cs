using MentorReservation.Api.Models;

namespace MentorReservation.Api.DTOs;

public record UserDto(int Id, string FullName, string Email, UserRole Role);
