using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Graph;
using Microsoft.Kiota.Abstractions.Authentication;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TimeRecorderBACKEND.DataBaseContext;
using TimeRecorderBACKEND.Dtos;
using TimeRecorderBACKEND.Services;
using Xunit;

public class UserServiceTests
{
    [Fact]
    public async Task GetAllAsync_ReturnsUsers()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTestDb1")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            context.Users.Add(new TimeRecorderBACKEND.Models.User
            {
                Id = Guid.NewGuid(),
                Name = "Jan",
                Surname = "Kowalski",
                Email = "jan@kowalski.com",
                ExistenceStatus = TimeRecorderBACKEND.Enums.ExistenceStatus.Exist
            });
            context.SaveChanges();

            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            Mock<IAuthenticationProvider> authProviderMock = new Mock<IAuthenticationProvider>();
            GraphServiceClient graphClient = new GraphServiceClient(authProviderMock.Object);

            UserService service = new UserService(graphClient, context, emailServiceMock.Object, httpContextAccessorMock.Object);

            IEnumerable<UserDto> users = await service.GetAllAsync();

            Assert.Single(users);
            Assert.Equal("Jan", users.First().Name);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsUser_WhenExists()
    {
        Guid userId = Guid.NewGuid();
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTestDb2")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            context.Users.Add(new TimeRecorderBACKEND.Models.User
            {
                Id = userId,
                Name = "Anna",
                Surname = "Nowak",
                Email = "anna@nowak.com",
                ExistenceStatus = TimeRecorderBACKEND.Enums.ExistenceStatus.Exist
            });
            context.SaveChanges();

            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            Mock<IAuthenticationProvider> authProviderMock = new Mock<IAuthenticationProvider>();
            GraphServiceClient graphClient = new GraphServiceClient(authProviderMock.Object);

            UserService service = new UserService(graphClient, context, emailServiceMock.Object, httpContextAccessorMock.Object);

            UserDto user = await service.GetByIdAsync(userId);

            Assert.NotNull(user);
            Assert.Equal("Anna", user.Name);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotExists()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTestDb3")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            Mock<IAuthenticationProvider> authProviderMock = new Mock<IAuthenticationProvider>();
            GraphServiceClient graphClient = new GraphServiceClient(authProviderMock.Object);

            UserService service = new UserService(graphClient, context, emailServiceMock.Object, httpContextAccessorMock.Object);

            UserDto user = await service.GetByIdAsync(Guid.NewGuid());

            Assert.Null(user);
        }
    }

    [Fact]
    public async Task AssignProjectAsync_ReturnsFalse_WhenUserNotExist()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTestDb5")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            context.Projects.Add(new TimeRecorderBACKEND.Models.Project
            {
                Id = 1,
                Name = "ProjektY"
            });
            context.SaveChanges();

            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            Mock<IAuthenticationProvider> authProviderMock = new Mock<IAuthenticationProvider>();
            GraphServiceClient graphClient = new GraphServiceClient(authProviderMock.Object);

            UserService service = new UserService(graphClient, context, emailServiceMock.Object, httpContextAccessorMock.Object);

            bool result = await service.AssignProjectAsync(Guid.NewGuid(), 1);

            Assert.False(result);
        }
    }

    [Fact]
    public async Task AssignProjectAsync_ReturnsFalse_WhenProjectNotExist()
    {
        Guid userId = Guid.NewGuid();
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTestDb6")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            context.Users.Add(new TimeRecorderBACKEND.Models.User
            {
                Id = userId,
                Name = "Marek",
                Surname = "Lewandowski",
                Email = "marek@lewandowski.com",
                ExistenceStatus = TimeRecorderBACKEND.Enums.ExistenceStatus.Exist
            });
            context.SaveChanges();

            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            Mock<IAuthenticationProvider> authProviderMock = new Mock<IAuthenticationProvider>();
            GraphServiceClient graphClient = new GraphServiceClient(authProviderMock.Object);

            UserService service = new UserService(graphClient, context, emailServiceMock.Object, httpContextAccessorMock.Object);

            bool result = await service.AssignProjectAsync(userId, 999);

            Assert.False(result);
        }
    }

    [Fact]
    public async Task UnassignProjectAsync_UnassignsProject_WhenUserExists()
    {
        Guid userId = Guid.NewGuid();
        int projectId = 2;
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTestDb7")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            TimeRecorderBACKEND.Models.Project project = new TimeRecorderBACKEND.Models.Project
            {
                Id = projectId,
                Name = "ProjektZ"
            };
            context.Projects.Add(project);
            context.Users.Add(new TimeRecorderBACKEND.Models.User
            {
                Id = userId,
                Name = "Kasia",
                Surname = "Nowicka",
                Email = "kasia@nowicka.com",
                ExistenceStatus = TimeRecorderBACKEND.Enums.ExistenceStatus.Exist,
                Project = project,
                ProjectId = projectId
            });
            context.SaveChanges();

            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            Mock<IAuthenticationProvider> authProviderMock = new Mock<IAuthenticationProvider>();
            GraphServiceClient graphClient = new GraphServiceClient(authProviderMock.Object);

            UserService service = new UserService(graphClient, context, emailServiceMock.Object, httpContextAccessorMock.Object);

            bool result = await service.UnassignProjectAsync(userId);

            Assert.True(result);
            TimeRecorderBACKEND.Models.User user = context.Users.First(u => u.Id == userId);
            Assert.Null(user.Project);
            Assert.Null(user.ProjectId);
        }
    }

    [Fact]
    public async Task UnassignProjectAsync_ReturnsFalse_WhenUserNotExist()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTestDb8")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            Mock<IAuthenticationProvider> authProviderMock = new Mock<IAuthenticationProvider>();
            GraphServiceClient graphClient = new GraphServiceClient(authProviderMock.Object);

            UserService service = new UserService(graphClient, context, emailServiceMock.Object, httpContextAccessorMock.Object);

            bool result = await service.UnassignProjectAsync(Guid.NewGuid());

            Assert.False(result);
        }
    }

    [Fact]
    public async Task GetUserProjectAsync_ReturnsProject_WhenExists()
    {
        Guid userId = Guid.NewGuid();
        int projectId = 3;
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTestDb9")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            TimeRecorderBACKEND.Models.Project project = new TimeRecorderBACKEND.Models.Project
            {
                Id = projectId,
                Name = "ProjektA",
                Description = "Opis"
            };
            context.Projects.Add(project);
            context.Users.Add(new TimeRecorderBACKEND.Models.User
            {
                Id = userId,
                Name = "Tomasz",
                Surname = "Lis",
                Email = "tomasz@lis.com",
                ExistenceStatus = TimeRecorderBACKEND.Enums.ExistenceStatus.Exist,
                Project = project,
                ProjectId = projectId
            });
            context.SaveChanges();

            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            Mock<IAuthenticationProvider> authProviderMock = new Mock<IAuthenticationProvider>();
            GraphServiceClient graphClient = new GraphServiceClient(authProviderMock.Object);

            UserService service = new UserService(graphClient, context, emailServiceMock.Object, httpContextAccessorMock.Object);

            ProjectDto projectDto = await service.GetUserProjectAsync(userId);

            Assert.NotNull(projectDto);
            Assert.Equal(projectId, projectDto.Id);
            Assert.Equal("ProjektA", projectDto.Name);
        }
    }

    [Fact]
    public async Task GetUserProjectAsync_ReturnsNull_WhenUserOrProjectNotExist()
    {
        Guid userId = Guid.NewGuid();
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "UserServiceTestDb10")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
            Mock<IHttpContextAccessor> httpContextAccessorMock = new Mock<IHttpContextAccessor>();
            Mock<IAuthenticationProvider> authProviderMock = new Mock<IAuthenticationProvider>();
            GraphServiceClient graphClient = new GraphServiceClient(authProviderMock.Object);

            UserService service = new UserService(graphClient, context, emailServiceMock.Object, httpContextAccessorMock.Object);

            ProjectDto projectDto = await service.GetUserProjectAsync(userId);

            Assert.Null(projectDto);
        }
    }
}