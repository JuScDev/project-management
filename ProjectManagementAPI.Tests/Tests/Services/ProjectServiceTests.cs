using Xunit;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProjectManagementAPI.Models;
using ProjectManagementAPI.Repositories;
using ProjectManagementAPI.Services;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly ProjectService _projectService;

    public ProjectServiceTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _projectService = new ProjectService(_projectRepositoryMock.Object);
    }

    [Fact]
    public async Task GetProjectsByOwnerAsync_ShouldReturnProjects_WhenOwnerExists()
    {
        // Arrange
        var owner = "testuser";
        var projects = new List<Project>
        {
            new Project { Id = 1, Name = "Project 1", Owner = owner },
            new Project { Id = 2, Name = "Project 2", Owner = owner }
        };

        _projectRepositoryMock.Setup(repo => repo.GetProjectsByOwnerAsync(owner))
            .ReturnsAsync(projects);

        // Act
        var result = await _projectService.GetProjectsByOwnerAsync(owner);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetProjectByIdAsync_ShouldReturnProject_WhenProjectExists()
    {
        // Arrange
        var project = new Project { Id = 1, Name = "Test Project", Owner = "testuser" };

        _projectRepositoryMock.Setup(repo => repo.GetProjectByIdAsync(1))
            .ReturnsAsync(project);

        // Act
        var result = await _projectService.GetProjectByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ShouldReturnNull_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock.Setup(repo => repo.GetProjectByIdAsync(1))
            .ReturnsAsync((Project?)null);

        // Act
        var result = await _projectService.GetProjectByIdAsync(1);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProjectAsync_ShouldReturnProject_WhenCreatedSuccessfully()
    {
        // Arrange
        var project = new Project { Name = "New Project", Description = "Test Description" };
        var createdProject = new Project { Id = 1, Name = "New Project", Description = "Test Description", Owner = "testuser" };

        _projectRepositoryMock.Setup(repo => repo.CreateProjectAsync(It.IsAny<Project>()))
            .ReturnsAsync(createdProject);

        // Act
        var result = await _projectService.CreateProjectAsync(project, "testuser");

        // Assert
        Assert.NotNull(result);
        Assert.Equal("testuser", result.Owner);
        Assert.Equal("New Project", result.Name);
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldReturnTrue_WhenUpdateIsSuccessful()
    {
        // Arrange
        var updatedProject = new Project { Id = 1, Name = "Updated Project", Owner = "testuser" };

        _projectRepositoryMock.Setup(repo => repo.UpdateProjectAsync(1, updatedProject))
            .ReturnsAsync(true);

        // Act
        var result = await _projectService.UpdateProjectAsync(1, updatedProject);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldReturnFalse_WhenUpdateFails()
    {
        // Arrange
        var updatedProject = new Project { Id = 1, Name = "Updated Project", Owner = "testuser" };

        _projectRepositoryMock.Setup(repo => repo.UpdateProjectAsync(1, updatedProject))
            .ReturnsAsync(false);

        // Act
        var result = await _projectService.UpdateProjectAsync(1, updatedProject);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldReturnTrue_WhenDeletionIsSuccessful()
    {
        // Arrange
        _projectRepositoryMock.Setup(repo => repo.DeleteProjectAsync(1))
            .ReturnsAsync(true);

        // Act
        var result = await _projectService.DeleteProjectAsync(1);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldReturnFalse_WhenProjectDoesNotExist()
    {
        // Arrange
        _projectRepositoryMock.Setup(repo => repo.DeleteProjectAsync(1))
            .ReturnsAsync(false);

        // Act
        var result = await _projectService.DeleteProjectAsync(1);

        // Assert
        Assert.False(result);
    }
}
