namespace PdfMarket.Domain.Entities;

/// <summary>
/// Domain entity representing a user account.
/// </summary>
public class User
{
    /// <summary>
    /// Unique identifier for the user (stored as string for Mongo convenience).
    /// </summary>
    public string Id { get; set; } = Guid.NewGuid().ToString();

    /// <summary>
    /// Public username displayed in the UI.
    /// </summary>
    public string UserName { get; set; } = default!;

    /// <summary>
    /// Email address used for account identification.
    /// </summary>
    public string Email { get; set; } = default!;

    /// <summary>
    /// Password representation.
    /// Project scope: stored as-is. In production, store a salted hash.
    /// </summary>
    public string PasswordHash { get; set; } = default!;

    /// <summary>
    /// Role used for authorization decisions (e.g., "User" or "Admin").
    /// </summary>
    public string Role { get; set; } = "User";

    /// <summary>
    /// Current balance of points the user can spend.
    /// </summary>
    public int PointsBalance { get; set; } = 0;

    /// <summary>
    /// IDs of PDFs this user owns (has purchased).
    /// Used as a fast authorization/UX helper (not a replacement for purchase records).
    /// </summary>
    public HashSet<string> OwnedPdfIds { get; set; } = new();
}
