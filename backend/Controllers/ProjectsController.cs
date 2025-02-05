using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly ProjectDbContext _context;

    public ProjectsController(ProjectDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        var username = User?.Identity?.Name;
        if (username == null)
        {
            return Unauthorized("No user is currently logged in.");
        }

        var projects = await _context.Projects
         .Where(p => p.Owner == username)
         .ToListAsync();

        return Ok(projects);
    }

    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject(Project project)
    {
        var username = User?.Identity?.Name;
        if (username == null)
        {
            return Unauthorized("No user is currently logged in.");
        }

        project.Owner = username;
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetProjects), new { id = project.Id }, project);
    }
}
