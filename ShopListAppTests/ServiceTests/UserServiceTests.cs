using Moq;
using ShopListApp.Commands.CreateCommands;
using ShopListApp.Commands.UpdateCommands;
using ShopListApp.Exceptions;
using ShopListApp.Interfaces;
using ShopListApp.Models;
using ShopListApp.Repositories;
using ShopListApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopListAppTests.ServiceTests
{
    public class UserServiceTests
    {
        private Mock<IUserRepository> _mockRepo;
        private UserService _userService;
        public UserServiceTests()
        {
            _mockRepo = new Mock<IUserRepository>();
            _userService = new UserService(_mockRepo.Object);
        }

        [Fact]
        public void CreateUser_ValidUser_CreatesUser()
        {
            CreateUserCommand cmd = new CreateUserCommand
            {
                UserName = "test",
                Email = "test@gmail.com",
                Password = "Password123@"
            };
            _mockRepo.Setup(x => x.CreateUser(It.IsAny<User>(), cmd.Password)).Returns(Task.CompletedTask);

            var task = _userService.CreateUser(cmd);

            Assert.True(task.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task CreateUser_NullUser_ThrowsArgumentNullException()
        {
            CreateUserCommand cmd = null!;

            Func<Task> task = async () => await _userService.CreateUser(cmd);

            await Assert.ThrowsAsync<ArgumentNullException>(task);
        }

        [Fact]
        public async Task CreateUser_DatabaseError_ThrowsDatabaseErrorException()
        {
            CreateUserCommand cmd = new CreateUserCommand
            {
                UserName = "test",
                Email = "test@gmail.com",
                Password = "Password123@"
            };
            _mockRepo.Setup(x => x.CreateUser(It.IsAny<User>(), cmd.Password)).ThrowsAsync(new Exception());

            Func<Task> task = async () => await _userService.CreateUser(cmd);

            await Assert.ThrowsAsync<DatabaseErrorException>(task);
        }

        [Fact]
        public void DeleteUser_UserFound_DeletesUser()
        {
            string id = "1";
            _mockRepo.Setup(x => x.DeleteUser(id)).ReturnsAsync(true);

            var task = _userService.DeleteUser(id);

            Assert.True(task.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task DeleteUser_UserNotFound_ThrowsUnathorizedAccessException()
        {
            string id = "1";
            _mockRepo.Setup(x => x.DeleteUser(id)).ReturnsAsync(false);

            Func<Task> task = async () => await _userService.DeleteUser(id);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(task);
        }

        [Fact]
        public async Task DeleteUser_NullArg_ThrowsArgumentNullException()
        {
            string id = null!;

            Func<Task> task = async () => await _userService.DeleteUser(id);

            await Assert.ThrowsAsync<ArgumentNullException>(task);
        }

        [Fact]
        public async Task DeleteUser_DatabaseError_ThrowsDatabaseErrorException()
        {
            string id = "1";
            _mockRepo.Setup(x => x.DeleteUser(id)).ThrowsAsync(new Exception());

            Func<Task> task = async () => await _userService.DeleteUser(id);

            await Assert.ThrowsAsync<DatabaseErrorException>(task);
        }

        [Fact]
        public void UpdateUser_UserFound_UpdatesUser()
        {
            string id = "1";
            UpdateUserCommand cmd = new UpdateUserCommand
            {
                UserName = "updated",
                CurrentPassword = "Password123@"
            };
            _mockRepo.Setup(x => x.UpdateUser(id, It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(true);

            var task = _userService.UpdateUser(id, cmd);

            Assert.True(task.IsCompletedSuccessfully);
        }

        [Fact]
        public async Task UpdateUser_NotFoundOrInvalidPassword_ThrowsUnathorizedAccessException()
        {
            string id = "1";
            UpdateUserCommand cmd = new UpdateUserCommand
            {
                UserName = "updated",
                CurrentPassword = "Password123@"
            };
            _mockRepo.Setup(x => x.UpdateUser(id, It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(false);

            Func<Task> task = async () => await _userService.UpdateUser(id, cmd);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(task);
        }

        public static IEnumerable<object[]> GetNullArgsForUpdateUser()
        {
            yield return new object[] { null!, null! }; 
            yield return new object[] { "123", null! }; 
            yield return new object[] { null!, new UpdateUserCommand { UserName = "test", CurrentPassword = "Password123@" } }; 
        }

        [Theory]
        [MemberData(nameof(GetNullArgsForUpdateUser))]
        public async Task UpdateUser_NullArgs_ThrowsArgumentNullException(string? id, UpdateUserCommand? cmd)
        {
            Func<Task> task = async () => await _userService.UpdateUser(id!, cmd!);

            await Assert.ThrowsAsync<ArgumentNullException>(task);
        }

        [Fact]
        public async Task UpdateUser_DatabaseError_ThrowsDatabaseErrorException()
        {
            string id = "1";
            UpdateUserCommand cmd = new UpdateUserCommand
            {
                UserName = "updated",
                CurrentPassword = "Password123@"
            };
            _mockRepo.Setup(x => x.UpdateUser(id, It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).ThrowsAsync(new Exception());

            Func<Task> task = async () => await _userService.UpdateUser(id, cmd);

            await Assert.ThrowsAsync<DatabaseErrorException>(task);
        }

        [Fact]
        public async Task GetUserById_UserFound_ReturnsUser()
        {
            string id = "1";
            _mockRepo.Setup(x => x.GetUserById(id)).ReturnsAsync(new User { Id = "1" });

            var user = await _userService.GetUserById(id);

            Assert.True(user!.Id == id);
        }

        [Fact]
        public async Task GetUserById_UserNotFound_ThrowsUnathorizedAccessException()
        {
            string id = "1";
            _mockRepo.Setup(x => x.GetUserById(id)).ReturnsAsync((User?)null);

            Func<Task> task = async () => await _userService.GetUserById(id);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(task);
        }

        [Fact]
        public async Task GetUserById_DatabaseError_ThrowsDatabaseErrorException()
        {
            string id = "1";
            _mockRepo.Setup(x => x.GetUserById(id)).ThrowsAsync(new Exception());

            Func<Task> task = async () => await _userService.GetUserById(id);

            await Assert.ThrowsAsync<DatabaseErrorException>(task);
        }

        [Fact]
        public async Task GetUserById_NullArg_ThrowsArgumentNullException()
        {
            string id = null!;

            Func<Task> task = async () => await _userService.GetUserById(id);

            await Assert.ThrowsAsync<ArgumentNullException>(task);
        }
    }
}
