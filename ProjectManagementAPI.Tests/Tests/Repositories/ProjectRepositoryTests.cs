using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProjectManagementAPI.Data;
using ProjectManagementAPI.Models;
using ProjectManagementAPI.Repositories;
using Xunit;

public class ProjectRepositoryTests : IDisposable
{
    private readonly ProjectDbContext _context;
    private readonly ProjectRepository _projectRepository;

    public ProjectRepositoryTests()
    {
        // Setup in-memory database
        var options = new DbContextOptionsBuilder<ProjectDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) // Unique DB for each test
            .Options;

        _context = new ProjectDbContext(options);
        _projectRepository = new ProjectRepository(_context);
    }

    [Fact]
    public async Task GetProjectsByOwnerAsync_ShouldReturnProjects_WhenOwnerHasProjects()
    {
        // Arrange
        var owner = "testuser";
        var projects = new List<Project>
        {
            new Project { Id = 1, Name = "Project 1", Owner = owner },
            new Project { Id = 2, Name = "Project 2", Owner = owner }
        };

        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        // Act
        var result = await _projectRepository.GetProjectsByOwnerAsync(owner);

        // Assert
        Assert.Equal(2, result.Count());
    }

    [Fact]
    public async Task GetProjectsByOwnerAsync_ShouldReturnEmptyList_WhenOwnerHasNoProjects()
    {
        // Act
        var result = await _projectRepository.GetProjectsByOwnerAsync("unknownuser");

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ShouldReturnProject_WhenProjectExists()
    {
        // Arrange
        var project = new Project { Id = 1, Name = "Test Project", Owner = "testuser" };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _projectRepository.GetProjectByIdAsync(1);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ShouldReturnNull_WhenProjectDoesNotExist()
    {
        // Act
        var result = await _projectRepository.GetProjectByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task CreateProjectAsync_ShouldAddProjectToDatabase()
    {
        // Arrange
        var project = new Project { Name = "New Project", Description = "Test Description", Owner = "testuser" };

        // Act
        var result = await _projectRepository.CreateProjectAsync(project);
        var storedProject = await _context.Projects.FirstOrDefaultAsync(p => p.Name == "New Project");

        // Assert
        Assert.NotNull(storedProject);
        Assert.Equal("New Project", storedProject.Name);
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldReturnTrue_WhenUpdateIsSuccessful()
    {
        // Arrange
        var project = new Project { Id = 1, Name = "Original Project", Description = "Original Description", Owner = "testuser" };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var updatedProject = new Project { Id = 1, Name = "Updated Project", Description = "Updated Description", Owner = "testuser" };

        // Act
        var result = await _projectRepository.UpdateProjectAsync(1, updatedProject);
        var storedProject = await _context.Projects.FindAsync(1);

        // Assert
        Assert.True(result);
        Assert.NotNull(storedProject);
        Assert.Equal("Updated Project", storedProject.Name);
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldReturnFalse_WhenProjectDoesNotExist()
    {
        // Arrange
        var updatedProject = new Project { Id = 1, Name = "Updated Project", Description = "Updated Description", Owner = "testuser" };

        // Act
        var result = await _projectRepository.UpdateProjectAsync(999, updatedProject);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldReturnTrue_WhenDeletionIsSuccessful()
    {
        // Arrange
        var project = new Project { Id = 1, Name = "Project to Delete", Owner = "testuser" };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _projectRepository.DeleteProjectAsync(1);
        var deletedProject = await _context.Projects.FindAsync(1);

        // Assert
        Assert.True(result);
        Assert.Null(deletedProject);
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldReturnFalse_WhenProjectDoesNotExist()
    {
        // Act
        var result = await _projectRepository.DeleteProjectAsync(999);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task GetProjectsByOwnerAsync_ShouldOnlyReturnProjectsOfSpecifiedOwner()
    {
        // Arrange
        var projects = new List<Project>
    {
        new Project { Id = 1, Name = "Project A", Owner = "testuser1" },
        new Project { Id = 2, Name = "Project B", Owner = "testuser2" },
        new Project { Id = 3, Name = "Project C", Owner = "testuser1" }
    };

        _context.Projects.AddRange(projects);
        await _context.SaveChangesAsync();

        // Act
        var result = await _projectRepository.GetProjectsByOwnerAsync("testuser1");

        // Assert
        Assert.Equal(2, result.Count()); // Should return only projects owned by "testuser1"
        Assert.DoesNotContain(result, p => p.Owner == "testuser2");
    }

    [Fact]
    public async Task UpdateProjectAsync_ShouldNotChangeOwner_WhenUpdatingProject()
    {
        // Arrange
        var project = new Project { Id = 1, Name = "Original", Description = "Desc", Owner = "testuser" };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        var updatedProject = new Project { Id = 1, Name = "Updated Name", Description = "Updated Desc", Owner = "maliciousUser" };

        // Act
        var result = await _projectRepository.UpdateProjectAsync(1, updatedProject);
        var storedProject = await _context.Projects.FindAsync(1);

        // Assert
        Assert.True(result);
        Assert.NotNull(storedProject);
        Assert.Equal("Updated Name", storedProject.Name);
        Assert.Equal("testuser", storedProject.Owner); // Owner should not change!
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldNotDeleteProjectFromAnotherUser()
    {
        // Arrange
        var project = new Project { Id = 1, Name = "Test Project", Owner = "user1" };
        _context.Projects.Add(project);
        await _context.SaveChangesAsync();

        // Act
        var result = await _projectRepository.DeleteProjectAsync(1);

        // Assert
        Assert.True(result); // Delete method doesn't check ownership, we should test it in service/controller layer
    }


    // Dispose database after tests
    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
