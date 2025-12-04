namespace PdfMarket.Domain.Entities;

public class User
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string UserName { get; set; } = default!;
    public string Email { get; set; } = default!;

    // TODO: replace with a proper password hash later
    public string PasswordHash { get; set; } = default!;

    /// <summary>
    /// "User" or "Admin" (for role-based access).
    /// </summary>
    public string Role { get; set; } = "User";

    /// <summary>
    /// Current balance of points the user can spend.
    /// </summary>
    public int PointsBalance { get; set; } = 0;

    /// <summary>
    /// IDs of PDFs this user owns (has purchased).
    /// </summary>
    public HashSet<string> OwnedPdfIds { get; set; } = new();
}
