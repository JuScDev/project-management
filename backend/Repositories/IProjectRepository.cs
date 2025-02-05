using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Repositories
{
    /// <summary>
    /// Interface for project repository.
    /// </summary>
    public interface IProjectRepository
    {
        /// <summary>
        /// Gets the projects by owner asynchronously.
        /// </summary>
        /// <param name="owner">The owner of the projects.</param>
        /// <returns>A list of projects owned by the specified owner.</returns>
        Task<IEnumerable<Project>> GetProjectsByOwnerAsync(string owner);

        /// <summary>
        /// Creates a new project asynchronously.
        /// </summary>
        /// <param name="project">The project to create.</param>
        /// <returns>The created project.</returns>
        Task<Project> CreateProjectAsync(Project project);
    }
}
