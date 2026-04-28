using MentorReservation.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace MentorReservation.Api.Data;

public static class SeedData
{
    public static async Task InitializeAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync())
        {
            return;
        }

        var mentors = new[]
        {
            new
            {
                User = new AppUser { FullName = "doc. dr. Ana Novak", Email = "ana.novak@fri.uni-lj.si", Role = UserRole.Mentor },
                Laboratory = "Laboratory for Data Technologies",
                Areas = new List<string> { "Data mining", "Databases", "Business intelligence" }
            },
            new
            {
                User = new AppUser { FullName = "izr. prof. dr. Marko Horvat", Email = "marko.horvat@fri.uni-lj.si", Role = UserRole.Mentor },
                Laboratory = "Laboratory for Artificial Intelligence",
                Areas = new List<string> { "Machine learning", "Natural language processing", "Recommendation systems" }
            },
            new
            {
                User = new AppUser { FullName = "prof. dr. Petra Zupan", Email = "petra.zupan@fri.uni-lj.si", Role = UserRole.Mentor },
                Laboratory = "Computer Communications Laboratory",
                Areas = new List<string> { "Computer networks", "Cybersecurity", "Distributed systems" }
            },
            new
            {
                User = new AppUser { FullName = "asist. dr. Luka Kovač", Email = "luka.kovac@fri.uni-lj.si", Role = UserRole.Mentor },
                Laboratory = "Software Engineering Laboratory",
                Areas = new List<string> { "Software engineering", "Web applications", "Process improvement" }
            }
        };

        foreach (var mentor in mentors)
        {
            db.Users.Add(mentor.User);
            db.MentorProfiles.Add(new MentorProfile
            {
                User = mentor.User,
                Laboratory = mentor.Laboratory,
                ResearchAreas = mentor.Areas,
                MaxStudents = 5
            });
        }

        var dino = new AppUser { FullName = "Dino Džaferagić", Email = "dino.dzaferagic@student.uni-lj.si", Role = UserRole.Student };
        var tibor = new AppUser { FullName = "Tibor Koderman", Email = "tibor.koderman@student.uni-lj.si", Role = UserRole.Student };
        var jerneja = new AppUser { FullName = "Jerneja Krajcar", Email = "jerneja.krajcar@student.uni-lj.si", Role = UserRole.Student };
        var maja = new AppUser { FullName = "Maja Vidmar", Email = "maja.vidmar@student.uni-lj.si", Role = UserRole.Student };

        db.Users.AddRange(dino, tibor, jerneja, maja);
        await db.SaveChangesAsync();

        var aiMentor = await db.MentorProfiles.SingleAsync(profile => profile.User.Email == "marko.horvat@fri.uni-lj.si");
        var processMentor = await db.MentorProfiles.SingleAsync(profile => profile.User.Email == "luka.kovac@fri.uni-lj.si");
        var securityMentor = await db.MentorProfiles.SingleAsync(profile => profile.User.Email == "petra.zupan@fri.uni-lj.si");

        db.MentorshipRequests.AddRange(
            new MentorshipRequest
            {
                StudentId = dino.Id,
                MentorId = processMentor.Id,
                ProposedTitle = "Mentor Selection System for Bachelor Thesis Supervision",
                Description = "A prototype for transparent mentor availability, structured student requests, and mentor-side request handling.",
                OptionalMessage = "This topic connects well with business process analysis and software engineering.",
                Status = RequestStatus.Accepted,
                MentorComment = "Good scope for a practical seminar prototype.",
                CreatedAt = DateTime.UtcNow.AddDays(-8),
                UpdatedAt = DateTime.UtcNow.AddDays(-7)
            },
            new MentorshipRequest
            {
                StudentId = tibor.Id,
                MentorId = aiMentor.Id,
                ProposedTitle = "Recommendation Support for Thesis Topic Discovery",
                Description = "Explore a simple recommendation approach for matching student interests with mentor research areas.",
                OptionalMessage = "I am interested in NLP and recommender systems.",
                Status = RequestStatus.Pending,
                CreatedAt = DateTime.UtcNow.AddDays(-2),
                UpdatedAt = DateTime.UtcNow.AddDays(-2)
            },
            new MentorshipRequest
            {
                StudentId = jerneja.Id,
                MentorId = securityMentor.Id,
                ProposedTitle = "Security Checklist for Student Web Applications",
                Description = "Prepare and evaluate a lightweight security checklist for common bachelor project web applications.",
                OptionalMessage = "I would like to focus on practical security improvements.",
                Status = RequestStatus.Rejected,
                MentorComment = "The idea is useful, but I cannot take this topic this semester.",
                CreatedAt = DateTime.UtcNow.AddDays(-5),
                UpdatedAt = DateTime.UtcNow.AddDays(-4)
            }
        );

        await db.SaveChangesAsync();
    }
}
