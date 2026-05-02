using HtmlAgilityPack;
using MentorReservation.Api.Data;
using MentorReservation.Api.DTOs;
using MentorReservation.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Text.RegularExpressions;

namespace MentorReservation.Api.Services;

public class MentorImportService(AppDbContext db, IWebHostEnvironment environment, HttpClient httpClient)
{
    private const string PublicSourceUrl = "https://www.fri.uni-lj.si/sl/mentorji";
    private const string LocalFileName = "https___www.fri.uni-lj.si_sl_mentorji.htm";
    private const string SourceName = "FRI mentorji page";

    public async Task<MentorImportRunDto> ImportAsync()
    {
        var startedAt = DateTime.UtcNow;
        var source = await LoadSourceAsync();
        var run = new MentorImportRun
        {
            SourceUrl = source.SourceUrl,
            StartedAt = startedAt
        };

        db.MentorImportRuns.Add(run);
        await db.SaveChangesAsync();

        try
        {
            var parsedMentors = ParseMentors(source.Html);
            var imported = 0;
            var updated = 0;
            var skipped = 0;

            foreach (var parsed in parsedMentors)
            {
                if (string.IsNullOrWhiteSpace(parsed.FirstName) || string.IsNullOrWhiteSpace(parsed.LastName))
                {
                    skipped++;
                    continue;
                }

                var profile = await FindExistingMentorAsync(parsed);
                var now = DateTime.UtcNow;

                if (profile is null)
                {
                    profile = new MentorProfile
                    {
                        CreatedAt = now,
                        MaxStudents = 5,
                        IsAvailable = true
                    };

                    db.MentorProfiles.Add(profile);
                    imported++;
                }
                else
                {
                    updated++;
                }

                profile.FirstName = parsed.FirstName;
                profile.LastName = parsed.LastName;
                profile.Title = parsed.Title;
                profile.Email = parsed.Email;
                profile.ProfileUrl = parsed.ProfileUrl;
                profile.Source = SourceName;
                profile.SourceExternalId = parsed.SourceExternalId;
                profile.ImportedAt = now;
                profile.UpdatedAt = now;

                await db.SaveChangesAsync();
                await ReplaceResearchAreasAsync(profile, parsed.ResearchAreas);
            }

            run.Status = ImportRunStatus.Success;
            run.ImportedCount = imported;
            run.UpdatedCount = updated;
            run.SkippedCount = skipped;
            run.FinishedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            run.Status = ImportRunStatus.Failed;
            run.ErrorMessage = ex.Message;
            run.FinishedAt = DateTime.UtcNow;
            await db.SaveChangesAsync();
        }

        return ToDto(run);
    }

    public async Task<List<MentorImportRunDto>> GetRunsAsync()
    {
        return await db.MentorImportRuns
            .AsNoTracking()
            .OrderByDescending(run => run.StartedAt)
            .Select(run => ToDto(run))
            .ToListAsync();
    }

    private async Task<SourceDocument> LoadSourceAsync()
    {
        var rootLocalFile = Path.GetFullPath(Path.Combine(environment.ContentRootPath, "..", LocalFileName));
        var backendLocalFile = Path.Combine(environment.ContentRootPath, LocalFileName);

        if (File.Exists(rootLocalFile))
        {
            return new SourceDocument(rootLocalFile, await File.ReadAllTextAsync(rootLocalFile));
        }

        if (File.Exists(backendLocalFile))
        {
            return new SourceDocument(backendLocalFile, await File.ReadAllTextAsync(backendLocalFile));
        }

        return new SourceDocument(PublicSourceUrl, await httpClient.GetStringAsync(PublicSourceUrl));
    }

    private static List<ParsedMentor> ParseMentors(string html)
    {
        var rawHtml = NormalizeSourceHtml(html);
        var document = new HtmlDocument();
        document.LoadHtml(rawHtml);

        var mentorNodes = document.DocumentNode.SelectNodes(
            "//li[.//a[contains(@href, '/sl/o-fakulteti/osebje/') or contains(@href, 'fri.uni-lj.si/sl/o-fakulteti/osebje/')]]");

        if (mentorNodes is null)
        {
            return [];
        }

        return mentorNodes
            .Select(ParseMentorNode)
            .Where(mentor => mentor is not null)
            .Select(mentor => mentor!)
            .GroupBy(mentor => mentor.ProfileUrl ?? $"{mentor.FirstName}|{mentor.LastName}", StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();
    }

    private static string NormalizeSourceHtml(string html)
    {
        if (!html.Contains("id=\"viewsource\"", StringComparison.OrdinalIgnoreCase) &&
            !html.Contains("resource://content-accessible/viewsource.css", StringComparison.OrdinalIgnoreCase))
        {
            return html;
        }

        var wrapper = new HtmlDocument();
        wrapper.LoadHtml(html);
        var body = wrapper.DocumentNode.SelectSingleNode("//body[@id='viewsource']") ?? wrapper.DocumentNode;
        return WebUtility.HtmlDecode(body.InnerText);
    }

    private static ParsedMentor? ParseMentorNode(HtmlNode li)
    {
        var anchor = li.SelectSingleNode(".//a[contains(@href, '/sl/o-fakulteti/osebje/') or contains(@href, 'fri.uni-lj.si/sl/o-fakulteti/osebje/')]");

        if (anchor is null)
        {
            return null;
        }

        var displayName = CleanText(anchor.InnerText);
        var profileUrl = NormalizeFriUrl(anchor.GetAttributeValue("href", string.Empty));

        if (string.IsNullOrWhiteSpace(displayName) || string.IsNullOrWhiteSpace(profileUrl))
        {
            return null;
        }

        var (title, firstName, lastName) = SplitTitleAndName(displayName);

        return new ParsedMentor(
            title,
            firstName,
            lastName,
            null,
            profileUrl,
            ExtractSourceExternalId(profileUrl),
            ExtractResearchAreas(CleanText(li.InnerText), displayName));
    }

    private async Task<MentorProfile?> FindExistingMentorAsync(ParsedMentor parsed)
    {
        if (!string.IsNullOrWhiteSpace(parsed.Email))
        {
            var normalizedEmail = parsed.Email.Trim().ToLowerInvariant();
            var byEmail = await db.MentorProfiles
                .Include(profile => profile.MentorResearchAreas)
                .SingleOrDefaultAsync(profile => profile.Email != null && profile.Email.ToLower() == normalizedEmail);

            if (byEmail is not null)
            {
                return byEmail;
            }
        }

        return await db.MentorProfiles
            .Include(profile => profile.MentorResearchAreas)
            .SingleOrDefaultAsync(profile =>
                profile.ProfileUrl == parsed.ProfileUrl ||
                (profile.SourceExternalId != null && profile.SourceExternalId == parsed.SourceExternalId));
    }

    private async Task ReplaceResearchAreasAsync(MentorProfile profile, IReadOnlyList<string> areaNames)
    {
        var existingLinks = await db.MentorResearchAreas
            .Where(link => link.MentorProfileId == profile.Id)
            .ToListAsync();

        db.MentorResearchAreas.RemoveRange(existingLinks);
        await db.SaveChangesAsync();

        foreach (var areaName in areaNames.Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var normalizedName = areaName.Trim();

            if (string.IsNullOrWhiteSpace(normalizedName))
            {
                continue;
            }

            var area = await db.ResearchAreas.SingleOrDefaultAsync(item => item.Name.ToLower() == normalizedName.ToLower());

            if (area is null)
            {
                area = new ResearchArea { Name = normalizedName, CreatedAt = DateTime.UtcNow };
                db.ResearchAreas.Add(area);
                await db.SaveChangesAsync();
            }

            db.MentorResearchAreas.Add(new MentorResearchArea
            {
                MentorProfileId = profile.Id,
                ResearchAreaId = area.Id
            });
        }

        await db.SaveChangesAsync();
    }

    private static (string? Title, string FirstName, string LastName) SplitTitleAndName(string displayName)
    {
        var name = CleanText(displayName)
            .Replace("prof .", "prof.", StringComparison.OrdinalIgnoreCase)
            .Replace("izr prof", "izr. prof", StringComparison.OrdinalIgnoreCase);
        var tokens = name.Split(' ', StringSplitOptions.RemoveEmptyEntries).ToList();
        var titleTokens = new List<string>();

        while (tokens.Count > 0 && IsTitleToken(tokens[0]))
        {
            titleTokens.Add(tokens[0]);
            tokens.RemoveAt(0);
        }

        if (tokens.Count == 0)
        {
            return (titleTokens.Count > 0 ? string.Join(' ', titleTokens) : null, name, string.Empty);
        }

        var firstName = tokens[0];
        var lastName = tokens.Count > 1 ? string.Join(' ', tokens.Skip(1)) : string.Empty;
        return (titleTokens.Count > 0 ? string.Join(' ', titleTokens) : null, firstName, lastName);
    }

    private static bool IsTitleToken(string token)
    {
        var normalized = token.Trim().Trim('.').ToLowerInvariant();
        return normalized is "akad" or "prof" or "izr" or "doc" or "dr" or "asist" or "red" or "mag";
    }

    private static IReadOnlyList<string> ExtractResearchAreas(string fullText, string displayName)
    {
        var text = fullText.Replace(displayName, string.Empty, StringComparison.OrdinalIgnoreCase);
        var matches = Regex.Matches(text, @"\((?<areas>[^)]{2,})\)");
        var areaText = matches.Count == 0 ? string.Empty : matches[^1].Groups["areas"].Value;

        return areaText
            .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(CleanText)
            .Where(area => area.Length > 1 && area.Length <= 180)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string NormalizeFriUrl(string url)
    {
        var value = WebUtility.HtmlDecode(url).Trim();
        value = value.StartsWith("view-source:", StringComparison.OrdinalIgnoreCase)
            ? value["view-source:".Length..]
            : value;

        if (value.StartsWith("https://fri.uni-lj.si", StringComparison.OrdinalIgnoreCase))
        {
            value = value.Replace("https://fri.uni-lj.si", "https://www.fri.uni-lj.si", StringComparison.OrdinalIgnoreCase);
        }

        if (value.StartsWith("/"))
        {
            value = $"https://www.fri.uni-lj.si{value}";
        }

        return value;
    }

    private static string ExtractSourceExternalId(string profileUrl)
    {
        var uri = new Uri(profileUrl);
        return uri.AbsolutePath.Trim('/').Split('/').Last();
    }

    private static string CleanText(string text) =>
        Regex.Replace(WebUtility.HtmlDecode(text), @"\s+", " ").Trim();

    private static MentorImportRunDto ToDto(MentorImportRun run) =>
        new(
            run.Id,
            run.SourceUrl,
            run.StartedAt,
            run.FinishedAt,
            run.Status,
            run.ImportedCount,
            run.UpdatedCount,
            run.SkippedCount,
            run.ErrorMessage);

    private record SourceDocument(string SourceUrl, string Html);

    private record ParsedMentor(
        string? Title,
        string FirstName,
        string LastName,
        string? Email,
        string ProfileUrl,
        string SourceExternalId,
        IReadOnlyList<string> ResearchAreas);
}
