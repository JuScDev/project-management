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
        public async Task<Project> CreateProjectAsync(Project project)
        {
            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            return project;
        }
    }
}
