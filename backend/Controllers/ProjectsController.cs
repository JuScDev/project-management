using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Services;
using ProjectManagementAPI.Models;

namespace ProjectManagementAPI.Controllers;

[Route("api/[controller]")]
[Authorize]
[ApiController]
public class ProjectsController : ControllerBase
{
    private readonly IProjectService _projectService;

    public ProjectsController(IProjectService projectService)
    {
        _projectService = projectService;
    }

    /// <summary>
    /// Retrieves projects owned by the currently logged-in user.
    /// </summary>
    /// <returns>A list of projects owned by the current user.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
    {
        var username = User?.Identity?.Name;
        if (username == null)
        {
            return Unauthorized("No user is currently logged in.");
        }

        var projects = await _projectService.GetProjectsByOwnerAsync(username);
        return Ok(projects);
    }

    /// <summary>
    /// Creates a new project for the currently logged-in user.
    /// </summary>
    /// <param name="project">The project to be created.</param>
    /// <returns>The created project.</returns>
    [HttpPost]
    public async Task<ActionResult<Project>> CreateProject(Project project)
    {
        var username = User?.Identity?.Name;
        if (username == null)
        {
            return Unauthorized("No user is currently logged in.");
        }

        var createdProject = await _projectService.CreateProjectAsync(project, username);
        return CreatedAtAction(nameof(GetProjects), new { id = createdProject.Id }, createdProject);
    }
}
