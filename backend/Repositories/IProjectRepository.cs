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
        /// Gets a project by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the project.</param>
        /// <returns>The project with the specified ID.</returns>
        Task<Project?> GetProjectByIdAsync(int id);

        /// <summary>
        /// Creates a new project asynchronously.
        /// </summary>
        /// <param name="project">The project to create.</param>
        /// <returns>The created project.</returns>
        Task<Project> CreateProjectAsync(Project project);

        /// <summary>
        /// Updates an existing project asynchronously.
        /// </summary>
        /// <param name="id">The ID of the project to update.</param>
        /// <param name="project">The updated project details.</param>
        /// <returns>The updated project.</returns>
        Task<bool> UpdateProjectAsync(int id, Project project);

        /// <summary>
        /// Deletes a project by its ID asynchronously.
        /// </summary>
        /// <param name="id">The ID of the project to delete.</param>
        /// <returns>The deleted project.</returns>
        Task<bool> DeleteProjectAsync(int id);
    }
}
