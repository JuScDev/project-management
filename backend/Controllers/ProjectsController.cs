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
    /// Retrieves a specific project by ID.
    /// </summary>
    /// <param name="id">The project ID.</param>
    /// <returns>The requested project if found.</returns>
    [HttpGet("{id}")]
    public async Task<ActionResult<Project>> GetProjectById(int id)
    {
        var username = User?.Identity?.Name;
        if (username == null)
        {
            return Unauthorized("No user is currently logged in.");
        }

        var project = await _projectService.GetProjectByIdAsync(id);
        if (project == null)
        {
            return NotFound("Project not found.");
        }

        if (project.Owner != username)
        {
            return Forbid("You do not have permission to load this project.");
        }

        return Ok(project);
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

    /// <summary>
    /// Updates an existing project.
    /// </summary>
    /// <param name="id">The ID of the project to update.</param>
    /// <param name="project">The updated project data.</param>
    /// <returns>No content if successful, or an error.</returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProject(int id, Project project)
    {
        var username = User?.Identity?.Name;
        if (username == null)
        {
            return Unauthorized("No user is currently logged in.");
        }

        var existingProject = await _projectService.GetProjectByIdAsync(id);
        if (existingProject == null)
        {
            return NotFound("Project not found.");
        }

        if (existingProject.Owner != username)
        {
            return Forbid("You do not have permission to delete this project.");
        }

        await _projectService.UpdateProjectAsync(id, project);
        return NoContent();
    }

    /// <summary>
    /// Deletes an existing project.
    /// </summary>
    /// <param name="id">The ID of the project to delete.</param>
    /// <returns>No content if successful, or an error.</returns>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProject(int id)
    {
        var username = User?.Identity?.Name;
        if (username == null)
        {
            return Unauthorized("No user is currently logged in.");
        }

        var existingProject = await _projectService.GetProjectByIdAsync(id);
        if (existingProject == null || existingProject.Owner != username)
        {
            return NotFound("Project not found or access denied.");
        }

        await _projectService.DeleteProjectAsync(id);
        return NoContent();
    }
}
