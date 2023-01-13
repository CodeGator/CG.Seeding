
namespace CG.Seeding;

/// <summary>
/// This class is a base implementation of the <see cref="ISeedDirectorBase"/>
/// interface.
/// </summary>
/// <typeparam name="T">The associated concrete seed director class.</typeparam>
public abstract class SeedDirectorBase<T> : ISeedDirectorBase
    where T : SeedDirectorBase<T>
{
    // *******************************************************************
    // Fields.
    // *******************************************************************

    #region Fields

    /// <summary>
    /// This field contains the logger for this director.
    /// </summary>
    internal protected readonly ILogger<T> _logger = null!;

    #endregion

    // *******************************************************************
    // Constructors.
    // *******************************************************************

    #region Constructors

    /// <summary>
    /// This constructor creates a new instance of the <see cref="SeedDirectorBase{T}"/>
    /// class.
    /// </summary>
    /// <param name="logger">The logger to use with this director.</param>
    protected SeedDirectorBase(
        ILogger<T> logger
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(logger, nameof(logger));

        // Save the reference(s).
        _logger = logger;
    }

    #endregion

    // *******************************************************************
    // Public methods.
    // *******************************************************************

    #region Public methods

    /// <inheritdoc/>
    public async virtual Task SeedAsync(
        List<string> fileNames, 
        string userName,
        bool force = false,
        CancellationToken cancellationToken = default
        )
    {
        // Validate the parameters before attempting to use them.
        Guard.Instance().ThrowIfNull(fileNames, nameof(fileNames))
            .ThrowIfNullOrEmpty(userName, nameof(userName));

        // Signal the start of the operation.
        await BeforeSeedAsync(
            userName,
            force,
            cancellationToken
            ).ConfigureAwait(false);

        // Log what we are about to do.
        _logger.LogDebug(
            "Seeding (count) JSON files.",
            fileNames.Count()
            );

        // Loop through the files.
        foreach (var fileName in fileNames)
        {
            // Log what we are about to do.
            _logger.LogDebug(
                "Parsing and validating file: '{file}'",
                fileName
                );

            // Read the configuration settings.
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(fileName, false);
            var configuration = builder.Build();

            // Log what we are about to do.
            _logger.LogDebug(
                "Calling SeedFromJson for file: '{file}'",
                fileName
                );

            // We have a convention that says the root value of this JSON
            //   file has the name of the type we're seeding.
            var rootValue = configuration.GetChildren().FirstOrDefault();

            // Did we fail?
            if (string.IsNullOrEmpty(rootValue?.Key))
            {
                // Panic!!
                throw new ArgumentException(
                    $"The root value is missing, or empty, in file: {fileName}"
                    );
            }

            // Defer to the derived class for the seeding operation.
            await SeedFromConfiguration(
                rootValue?.Key ?? "",
                configuration,
                userName,
                force,
                cancellationToken
                ).ConfigureAwait(false);
        }

        // Signal the end of the operation.
        await AfterSeedAsync(
            userName,
            force,
            cancellationToken
            ).ConfigureAwait(false);
    }

    #endregion

    // *******************************************************************
    // Protected methods.
    // *******************************************************************

    #region Protected methods

    /// <inheritdoc/>
    protected abstract Task SeedFromConfiguration(
        string objectName,
        IConfiguration dataSection,
        string userName,
        bool force = false,
        CancellationToken cancellationToken = default
        );

    // *******************************************************************

    /// <summary>
    /// This method is called before the seeding operation starts.
    /// </summary>
    /// <param name="force"><c>true</c> to force the seeding operation when data
    /// already exists in the associated table(s), <c>false</c> to stop the 
    /// operation whenever data is detected in the associated table(s).</param>
    /// <param name="userName">The user name of the person performing the 
    /// operation.</param>
    /// <param name="cancellationToken">A cancellation token that is monitored
    /// for the lifetime of the method.</param>
    /// <returns>A task to perform the operation.</returns>
    protected virtual Task BeforeSeedAsync(
        string userName,
        bool force = false,
        CancellationToken cancellationToken = default
        )
    {
        return Task.CompletedTask;
    }

    // *******************************************************************

    /// <summary>
    /// This method is called after the seeding operation completes.
    /// </summary>
    /// <param name="force"><c>true</c> to force the seeding operation when data
    /// already exists in the associated table(s), <c>false</c> to stop the 
    /// operation whenever data is detected in the associated table(s).</param>
    /// <param name="userName">The user name of the person performing the 
    /// operation.</param>
    /// <param name="cancellationToken">A cancellation token that is monitored
    /// for the lifetime of the method.</param>
    /// <returns>A task to perform the operation.</returns>
    protected virtual Task AfterSeedAsync(
        string userName,
        bool force = false,
        CancellationToken cancellationToken = default
        )
    {
        return Task.CompletedTask;
    }

    #endregion
}
