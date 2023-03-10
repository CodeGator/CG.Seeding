
namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// This class contains extension methods related to the <see cref="WebApplication"/>
/// type.
/// </summary>
public static class WebApplicationExtensions
{
    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method performs startup operations for the data seeding subsystem.
    /// </summary>
    /// <param name="webApplication">The web application to use for the 
    /// operation.</param>
    /// <returns>The value of the <paramref name="webApplication"/>
    /// parameter, for chaining calls together, Fluent style.</returns>
    public static WebApplication UseSeeding(
        this WebApplication webApplication
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(webApplication, nameof(webApplication));

        // Log what we are about to do.
        webApplication.Logger.LogDebug(
            "Checking the application's environment for the seeder."
            );

        // Log what we are about to do.
        webApplication.Logger.LogDebug(
            "Fetching the {op} options, for the seeder.",
            nameof(SeedingOptions)
            );

        // Get the seeding options.
        var seedingOptions = webApplication.Services.GetRequiredService<
            IOptions<SeedingOptions>
            >();

        // Should we skip seeding altogether?
        if (!seedingOptions.Value.SeedOnStartup)
        {
            // Log what we didn't do.
            webApplication.Logger.LogWarning(
                "Ignoring seeding startup because the SeedOnStartup flag, " +
                "in the {op} options, is either false, or missing. This " +
                "can also happen if the {name}' method hasn't been called " +
                "from the application's startup",
                nameof(SeedingOptions),
                nameof(WebApplicationBuilderExtensions.AddSeeding)
                );

            // Return the application.
            return webApplication;
        }

        // Sanity check the file names list.
        if (seedingOptions.Value.FileNames is null)
        {
            // Supply an empty list.
            seedingOptions.Value.FileNames = new();
        }

        // Log what we are about to do.
        webApplication.Logger.LogInformation(
            "Seeding from {count} JSON files, for the seeder.",
            seedingOptions.Value.FileNames.Count
            );

        // Log what we are about to do.
        webApplication.Logger.LogDebug(
            "Creating a DI scope, for the seeder."
            );

        // Create a DI scope.
        using var scope = webApplication.Services.CreateScope();

        // Log what we are about to do.
        webApplication.Logger.LogDebug(
            "Creating a seeding director instance, for the seeder."
            );

        // Get the seed director.
        var director = scope.ServiceProvider.GetRequiredService<ISeedDirectorBase>();

        // Log what we are about to do.
        webApplication.Logger.LogTrace(
            "Deferring to the {name} method, for the seeder",
            nameof(ISeedDirectorBase.SeedAsync)
            );

        // Perform the seeding operation(s).
        director.SeedAsync(
            seedingOptions.Value.FileNames,
            "seed",
            seedingOptions.Value.Force
            ).Wait();

        // Return the application.
        return webApplication;
    }

    #endregion
    
}
