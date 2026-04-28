using MentorReservation.Api.Data;
using MentorReservation.Api.DTOs;
using MentorReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorReservation.Api.Services;

public class UserService(AppDbContext db)
{
    public async Task<List<UserDto>> GetUsersAsync(UserRole? role)
    {
        var query = db.Users.AsNoTracking();

        if (role is not null)
        {
            query = query.Where(user => user.Role == role);
        }

        return await query
            .OrderBy(user => user.FullName)
            .Select(user => new UserDto(user.Id, user.FullName, user.Email, user.Role))
            .ToListAsync();
    }
}
