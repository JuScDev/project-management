using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagementAPI.Repositories
{
    /// <inheritdoc/>
    public class ProjectRepository : IProjectRepository
    {
        private readonly ProjectDbContext _context;

        /// <inheritdoc/>
        public ProjectRepository(ProjectDbContext context)
        {
            _context = context;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<Project>> GetProjectsByOwnerAsync(string owner)
        {
            return await _context.Projects
                .Where(p => p.Owner == owner)
                .ToListAsync();
        }

        /// <inheritdoc/>
        public async Task<Project?> GetProjectByIdAsync(int id)
        {
            return await _context.Projects.FindAsync(id);
        }

        /// <inheritdoc/>
        public async Task<Project> CreateProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }

        /// <inheritdoc/>
        public async Task<bool> UpdateProjectAsync(int id, Project project)
        {
            var existingProject = await _context.Projects.FindAsync(id);
            if (existingProject == null)
            {
                return false;
            }

            existingProject.Name = project.Name;
            existingProject.Description = project.Description;

            _context.Projects.Update(existingProject);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <inheritdoc/>
        public async Task<bool> DeleteProjectAsync(int id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return false;
            }

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
