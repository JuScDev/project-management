using ProjectManagementAPI.Models;
using ProjectManagementAPI.Repositories;

namespace ProjectManagementAPI.Services
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;

        public ProjectService(IProjectRepository projectRepository)
        {
            _projectRepository = projectRepository;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Project>> GetProjectsByOwnerAsync(string owner)
        {
            return await _projectRepository.GetProjectsByOwnerAsync(owner);
        }

        /// <inheritdoc/>
        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _projectRepository.GetProjectByIdAsync(id);
        }

        /// <inheritdoc/>
        public async Task<Project> CreateProjectAsync(Project project, string owner)
        {
            project.Owner = owner;
            return await _projectRepository.CreateProjectAsync(project);
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProjectAsync(int id, Project project)
        {
            return await _projectRepository.UpdateProjectAsync(id, project);
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteProjectAsync(int id)
        {
            return await _projectRepository.DeleteProjectAsync(id);
        }
    }
}
