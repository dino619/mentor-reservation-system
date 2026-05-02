namespace MentorReservation.Api.Models;

public class MentorResearchArea
{
    public int MentorProfileId { get; set; }
    public MentorProfile MentorProfile { get; set; } = null!;

    public int ResearchAreaId { get; set; }
    public ResearchArea ResearchArea { get; set; } = null!;
}
