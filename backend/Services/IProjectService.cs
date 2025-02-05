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
        /// Retrieves a project by its ID.
        /// </summary>
        /// <param name="id">The ID of the project.</param>
        /// <returns>The project with the specified ID, or null if not found.</returns>
        Task<Project?> GetProjectByIdAsync(int id);

        /// <summary>
        /// Creates a new project for a specific user.
        /// </summary>
        /// <param name="project">The project to be created.</param>
        /// <param name="owner">The username of the project owner.</param>
        /// <returns>The created project.</returns>
        Task<Project> CreateProjectAsync(Project project, string owner);

        /// <summary>
        /// Updates an existing project.
        /// </summary>
        /// <param name="id">The ID of the project to be updated.</param>
        /// <param name="project">The updated project details.</param>
        /// <returns>True if the update was successful, otherwise false.</returns>
        Task<bool> UpdateProjectAsync(int id, Project project);

        /// <summary>
        /// Deletes a project by its ID.
        /// </summary>
        /// <param name="id">The ID of the project to be deleted.</param>
        /// <returns>True if the deletion was successful, otherwise false.</returns>
        Task<bool> DeleteProjectAsync(int id);
    }
}
