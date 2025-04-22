using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using OneIncUserAPI;
using OneIncUserAPI.Controllers;
using OneIncUserAPI.Core.Application.Interfaces;
using OneIncUserAPI.Core.Domain.Models;
using OneIncUserAPI.Core.Application;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace TestOneIncUserAPI;

[TestClass]
public sealed class UserControllerTests
{
    private Mock<IApplicationRepository<AppUser>> _mockRepo;
    private Mock<ILogger<UserController>> _mockLogger;
    private UserController _controller;

    [TestInitialize]
    public void Setup()
    {
        _mockRepo = new Mock<IApplicationRepository<AppUser>>();
        _mockLogger = new Mock<ILogger<UserController>>();
        _controller = new UserController(_mockRepo.Object, _mockLogger.Object);
    }

    [TestMethod]
    public async Task GetOne_UserExists_ReturnsUser()
    {
        // Arrange
        var userId = "123";
        var user = new AppUser { Id = userId, FirstName = "John", LastName = "Doe" };
        _mockRepo.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Act
        var result = await _controller.GetOne(userId);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Data);
        Assert.IsInstanceOfType(result.Data, typeof(AppUser));
    }

    [TestMethod]
    public async Task GetOne_UserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var userId = "123";
        _mockRepo.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((AppUser?)null);

        // Act
        var result = await _controller.GetOne(userId);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(404, result.StatusCode);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task GetAll_UsersExist_ReturnsUsers()
    {
        // Arrange
        var users = new List<AppUser>
        {
            new AppUser { Id = "1", FirstName = "John", LastName = "Doe" },
            new AppUser { Id = "2", FirstName = "Jane", LastName = "Smith" }
        };
        _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(users);

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Data);
        Assert.IsInstanceOfType(result.Data, typeof(IEnumerable<AppUser>));
    }

    [TestMethod]
    public async Task GetAll_NoUsersExist_ReturnsNotFound()
    {
        // Arrange
        _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<AppUser>());

        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(404, result.StatusCode);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task AddUser_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var newUser = new AppUser { FirstName = "John", LastName = "Doe" };
        _mockRepo.Setup(repo => repo.AddAsync(newUser)).ReturnsAsync(newUser);

        // Act
        var result = await _controller.AddUser(newUser);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Data);
        Assert.IsInstanceOfType(result.Data, typeof(AppUser));
    }

    [TestMethod]
    public async Task AddUser_InvalidUser_ReturnsFailure()
    {
        // Arrange
        var newUser = new AppUser(); // Invalid user (e.g., missing required fields)
        _mockRepo.Setup(repo => repo.AddAsync(newUser)).ReturnsAsync(newUser);

        // Act
        var result = await _controller.AddUser(newUser);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(404, result.StatusCode);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task UpdateUser_ValidUser_ReturnsSuccess()
    {
        // Arrange
        var updatedUser = new AppUser { Id = "123", FirstName = "John", LastName = "Doe" };
        _mockRepo.Setup(repo => repo.UpdateAsync(updatedUser)).ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.UpdateUser(updatedUser);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Data);
        Assert.IsInstanceOfType(result.Data, typeof(AppUser));
    }

    [TestMethod]
    public async Task UpdateUser_InvalidUser_ReturnsFailure()
    {
        // Arrange
        var updatedUser = new AppUser(); // Invalid user
        _mockRepo.Setup(repo => repo.UpdateAsync(updatedUser)).ReturnsAsync(updatedUser);

        // Act
        var result = await _controller.UpdateUser(updatedUser);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(404, result.StatusCode);
        Assert.IsNull(result.Data);
    }

    [TestMethod]
    public async Task DeleteUser_UserExists_ReturnsSuccess()
    {
        // Arrange
        var userId = "123";
        var user = new AppUser { Id = userId, IsActive = true };
        _mockRepo.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);
        _mockRepo.Setup(repo => repo.DeleteAsync(user)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        Assert.IsTrue(result.Success);
        Assert.AreEqual(200, result.StatusCode);
        Assert.IsNotNull(result.Data);
        Assert.IsInstanceOfType(result.Data, typeof(AppUser));
    }

    [TestMethod]
    public async Task DeleteUser_UserDoesNotExist_ReturnsNotFound()
    {
        // Arrange
        var userId = "123";
        _mockRepo.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync((AppUser?)null);

        // Act
        var result = await _controller.DeleteUser(userId);

        // Assert
        Assert.IsFalse(result.Success);
        Assert.AreEqual(404, result.StatusCode);
        Assert.IsNull(result.Data);
    }
}
