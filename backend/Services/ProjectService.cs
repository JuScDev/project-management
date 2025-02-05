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
        public async Task<Project> CreateProjectAsync(Project project, string owner)
        {
            project.Owner = owner;
            return await _projectRepository.CreateProjectAsync(project);
        }
    }
}
