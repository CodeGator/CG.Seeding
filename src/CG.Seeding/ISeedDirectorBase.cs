
namespace CG.Seeding;

/// <summary>
/// This interface represents an object that performs data seeding operations.
/// </summary>
public interface ISeedDirectorBase
{
    /// <summary>
    /// This method performs a seeding operation based on the 
    /// content of the given <see cref="IConfiguration"/> object.
    /// </summary>
    /// <param name="fileNames">The collection of JSON files to use for the 
    /// operation.</param>
    /// <param name="userName">The user name of the person performing the 
    /// operation.</param>
    /// <param name="force"><c>true</c> to force the seeding operation when data
    /// already exists in the associated table(s), <c>false</c> to stop the 
    /// operation whenever data is detected in the associated table(s).</param>
    /// <param name="cancellationToken">A cancellation token that is monitored
    /// for the lifetime of the method.</param>
    /// <returns>A task to perform the operation.</returns>
    Task SeedAsync(
        List<string> fileNames,
        string userName,
        bool force = false,
        CancellationToken cancellationToken = default
        );
}
