using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Services
{
    public interface IProjectService
    {
        /// <summary>
        /// Retrieves projects owned by a specific user.
        /// </summary>
        /// <param name="owner">The username of the project owner.</param>
        /// <returns>A list of projects owned by the specified user.</returns>
        Task<IEnumerable<Project>> GetProjectsByOwnerAsync(string owner);

        /// <summary>
        /// Creates a new project for a specific user.
        /// </summary>
        /// <param name="project">The project to be created.</param>
        /// <param name="owner">The username of the project owner.</param>
        /// <returns>The created project.</returns>
        Task<Project> CreateProjectAsync(Project project, string owner);
    }
}
