
namespace Microsoft.AspNetCore.Builder;

/// <summary>
/// This class contains extension methods related to the <see cref="WebApplicationBuilder"/>
/// type.
/// </summary>
public static class WebApplicationBuilderExtensions
{
    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <summary>
    /// This method adds directors and related services, for data seeding 
    /// operations.
    /// </summary>
    /// <typeparam name="T">The type of associated concrete seed director
    /// to use for the operation.</typeparam>
    /// <param name="webApplicationBuilder">The web application builder to
    /// use for the operation.</param>
    /// <param name="sectionName">The configuration section to use for the 
    /// operation. Defaults to <c>Seeding</c>.</param>
    /// <param name="bootstrapLogger">A bootstrap logger to use for the
    /// operation.</param>
    /// <returns>The value of the <paramref name="webApplicationBuilder"/>
    /// parameter, for chaining calls together, Fluent style.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more arguments are missing, or invalid.</exception>
    public static WebApplicationBuilder AddSeeding<T>(
        this WebApplicationBuilder webApplicationBuilder,
        string sectionName = "Seeding",
        ILogger? bootstrapLogger = null
        ) where T : class, ISeedDirector
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(webApplicationBuilder, nameof(webApplicationBuilder))
            .ThrowIfNullOrEmpty(sectionName, nameof(sectionName));

        // Tell the world what we are about to do.
        bootstrapLogger?.LogDebug(
            "Configuring seeding options from the {section} section",
            sectionName
            );

        // Configure the seeding options.
        webApplicationBuilder.Services.ConfigureOptions<SeedingOptions>(
            webApplicationBuilder.Configuration.GetSection(sectionName),
            out var seedingOptions
            );

        // Tell the world what we are about to do.
        bootstrapLogger?.LogDebug(
            "Wiring up the concrete seed director: '{type}'",
            typeof(T).Name
            );

        // Add the director.
        webApplicationBuilder.Services.AddScoped<ISeedDirector, T>();

        // Return the application builder.
        return webApplicationBuilder;
    }

    // *******************************************************************

    /// <summary>
    /// This method adds directors and related services, for data seeding 
    /// operations.
    /// </summary>
    /// <typeparam name="T">The type of associated concrete seed director
    /// to use for the operation.</typeparam>
    /// <param name="webApplicationBuilder">The web application builder to
    /// use for the operation.</param>
    /// <param name="optionsDelegate">The delegate to use for configuring 
    /// the data seeding operations.</param>
    /// <param name="bootstrapLogger">A bootstrap logger to use for the
    /// operation.</param>
    /// <returns>The value of the <paramref name="webApplicationBuilder"/>
    /// parameter, for chaining calls together, Fluent style.</returns>
    /// <exception cref="ArgumentException">This exception is thrown whenever
    /// one or more arguments are missing, or invalid.</exception>
    public static WebApplicationBuilder AddSeeding<T>(
        this WebApplicationBuilder webApplicationBuilder,
        Action<SeedingOptions> optionsDelegate,
        ILogger? bootstrapLogger = null
        ) where T : class, ISeedDirector
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(webApplicationBuilder, nameof(webApplicationBuilder))
            .ThrowIfNull(optionsDelegate, nameof(optionsDelegate));

        // Tell the world what we are about to do.
        bootstrapLogger?.LogDebug(
            "Configuring seeding options from the caller"
            );

        // Give the caller a chance to configure the options.
        var options = new SeedingOptions();
        optionsDelegate?.Invoke(options);

        // Tell the world what we are about to do.
        bootstrapLogger?.LogDebug(
            "Validating seeding options"
            );

        // Validate the options.
        Guard.Instance().ThrowIfInvalidObject(options, nameof(options));

        // Tell the world what we are about to do.
        bootstrapLogger?.LogDebug(
            "Registering the seeding options"
            );

        // Add the options as a service.
        webApplicationBuilder.Services.AddSingleton<IOptions<SeedingOptions>>(
            new OptionsWrapper<SeedingOptions>(options)
            );

        // Tell the world what we are about to do.
        bootstrapLogger?.LogDebug(
            "Wiring up the concrete seed director: '{type}'",
            typeof(T).Name
            );

        // Add the director.
        webApplicationBuilder.Services.AddScoped<ISeedDirector, T>();

        // Return the application builder.
        return webApplicationBuilder;
    }

    #endregion
}


