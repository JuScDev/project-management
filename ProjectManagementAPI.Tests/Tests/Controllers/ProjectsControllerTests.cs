using Xunit;
using Moq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Controllers;
using ProjectManagementAPI.Models;
using ProjectManagementAPI.Services;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

public class ProjectsControllerTests
{
    private readonly Mock<IProjectService> _projectServiceMock;
    private readonly ProjectsController _projectsController;

    public ProjectsControllerTests()
    {
        _projectServiceMock = new Mock<IProjectService>();
        _projectsController = new ProjectsController(_projectServiceMock.Object);

        // Mock HttpContext for Authentication
        var httpContext = new DefaultHttpContext();
        var identity = new ClaimsIdentity(new List<Claim> { new Claim(ClaimTypes.Name, "testuser") });
        httpContext.User = new ClaimsPrincipal(identity);

        _projectsController.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task GetProjects_ShouldReturnOk_WhenUserIsAuthenticated()
    {
        // Arrange
        var mockProjects = new List<Project>
        {
            new Project { Id = 1, Name = "Project 1", Owner = "testuser" },
            new Project { Id = 2, Name = "Project 2", Owner = "testuser" }
        };

        _projectServiceMock
            .Setup(service => service.GetProjectsByOwnerAsync("testuser"))
            .ReturnsAsync(mockProjects);

        // Act
        var result = await _projectsController.GetProjects();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProjects = Assert.IsType<List<Project>>(okResult.Value);
        Assert.Equal(2, returnedProjects.Count);
    }

    [Fact]
    public async Task GetProjects_ShouldReturnUnauthorized_WhenUserIsNotAuthenticated()
    {
        // Arrange
        _projectsController.ControllerContext.HttpContext.User = new ClaimsPrincipal(); // No User-Claims

        // Act
        var result = await _projectsController.GetProjects();

        // Assert
        Assert.IsType<UnauthorizedObjectResult>(result.Result);
    }

    [Fact]
    public async Task GetProjectById_ShouldReturnOk_WhenProjectExistsAndBelongsToUser()
    {
        // Arrange
        var project = new Project { Id = 1, Name = "Test Project", Owner = "testuser" };

        _projectServiceMock
            .Setup(service => service.GetProjectByIdAsync(1))
            .ReturnsAsync(project);

        // Act
        var result = await _projectsController.GetProjectById(1);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        var returnedProject = Assert.IsType<Project>(okResult.Value);
        Assert.Equal(1, returnedProject.Id);
    }

    [Fact]
    public async Task GetProjectById_ShouldReturnNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectServiceMock
            .Setup(service => service.GetProjectByIdAsync(1))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _projectsController.GetProjectById(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result.Result);
    }

    [Fact]
    public async Task CreateProject_ShouldReturnCreated_WhenProjectIsSuccessfullyCreated()
    {
        // Arrange
        var newProject = new Project { Name = "New Project", Description = "Test Desc", Owner = "testuser" };
        var createdProject = new Project { Id = 1, Name = "New Project", Description = "Test Desc", Owner = "testuser" };

        _projectServiceMock
            .Setup(service => service.CreateProjectAsync(It.IsAny<Project>(), "testuser"))
            .ReturnsAsync(createdProject);

        // Act
        var result = await _projectsController.CreateProject(newProject);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result.Result);
        var returnedProject = Assert.IsType<Project>(createdResult.Value);
        Assert.Equal(1, returnedProject.Id);
    }

    [Fact]
    public async Task UpdateProject_ShouldReturnNoContent_WhenUpdateIsSuccessful()
    {
        // Arrange
        var existingProject = new Project { Id = 1, Name = "Test Project", Owner = "testuser" };
        var updatedProject = new Project { Id = 1, Name = "Updated Project", Owner = "testuser" };

        _projectServiceMock
            .Setup(service => service.GetProjectByIdAsync(1))
            .ReturnsAsync(existingProject);

        _projectServiceMock
            .Setup(service => service.UpdateProjectAsync(1, updatedProject))
            .ReturnsAsync(true);

        // Act
        var result = await _projectsController.UpdateProject(1, updatedProject);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task UpdateProject_ShouldReturnNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectServiceMock
            .Setup(service => service.GetProjectByIdAsync(1))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _projectsController.UpdateProject(1, new Project());

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }

    [Fact]
    public async Task DeleteProject_ShouldReturnNoContent_WhenDeletionIsSuccessful()
    {
        // Arrange
        var existingProject = new Project { Id = 1, Name = "Test Project", Owner = "testuser" };

        _projectServiceMock
            .Setup(service => service.GetProjectByIdAsync(1))
            .ReturnsAsync(existingProject);

        _projectServiceMock
            .Setup(service => service.DeleteProjectAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _projectsController.DeleteProject(1);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task DeleteProject_ShouldReturnNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectServiceMock
            .Setup(service => service.GetProjectByIdAsync(1))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _projectsController.DeleteProject(1);

        // Assert
        Assert.IsType<NotFoundObjectResult>(result);
    }
}
