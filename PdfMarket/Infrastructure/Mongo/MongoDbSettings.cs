namespace PdfMarket.Infrastructure.Mongo;

/// <summary>
/// MongoDB configuration settings loaded from appsettings.json.
/// </summary>
public class MongoDbSettings
{
    /// <summary>
    /// MongoDB connection string.
    /// </summary>
    public string ConnectionString { get; set; } = default!;

    /// <summary>
    /// Name of the MongoDB database used by the application.
    /// </summary>
    public string DatabaseName { get; set; } = default!;
}
