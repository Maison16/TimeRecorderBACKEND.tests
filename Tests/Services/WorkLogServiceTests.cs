using Xunit;
using Moq;
using TimeRecorderBACKEND.Services;
using TimeRecorderBACKEND.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TimeRecorderBACKEND.Enums;
using TimeRecorderBACKEND.Models;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TimeRecorderBACKEND.Dtos;

public class WorkLogServiceTests
{
    private WorkLogService CreateService(WorkTimeDbContext context, Guid? userId = null)
    {
        Mock<IEmailService> emailServiceMock = new Mock<IEmailService>();
        Mock<IUserService> userServiceMock = new Mock<IUserService>();
        if (userId.HasValue)
        {
            userServiceMock.Setup(x => x.GetByIdAsync(userId.Value))
                .ReturnsAsync(new UserDto { Id = userId.Value, Name = "Anna", Surname = "Nowak", Email = "anna@nowak.com" });
        }
        Mock<IHubContext<WorkStatusHub>> hubContextMock = new Mock<IHubContext<WorkStatusHub>>();
        Mock<IConfiguration> configMock = new Mock<IConfiguration>();
        configMock.Setup(x => x["AdminEmail"]).Returns("admin@admin.com");
        return new WorkLogService(context, emailServiceMock.Object, userServiceMock.Object, hubContextMock.Object, configMock.Object);
    }

    [Fact]
    public async Task GetSpecific_ReturnsWorkLogs()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase("GetSpecificDb").Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            Guid userId = Guid.NewGuid();
            context.Users.Add(new User { Id = userId, Name = "Anna", Surname = "Nowak", Email = "anna@nowak.com", ExistenceStatus = ExistenceStatus.Exist });
            context.WorkLogs.Add(new WorkLog { UserId = userId, StartTime = DateTime.Now, Type = WorkLogType.Work, ExistenceStatus = ExistenceStatus.Exist });
            context.SaveChanges();

            WorkLogService service = CreateService(context, userId);
            IEnumerable<WorkLogDtoWithUserNameAndSurname> result = await service.GetSpecific(userId);

            Assert.Single(result);
        }
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsNull_WhenNotFound()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase("GetByIdAsyncDb").Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            WorkLogService service = CreateService(context);
            WorkLogDto result = await service.GetByIdAsync(999);
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task UpdateAsync_UpdatesWorkLog()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase("UpdateAsyncDb").Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            WorkLog workLog = new WorkLog { Id = 1, UserId = Guid.NewGuid(), StartTime = DateTime.Now, Type = WorkLogType.Work, ExistenceStatus = ExistenceStatus.Exist };
            context.WorkLogs.Add(workLog);
            context.SaveChanges();

            WorkLogService service = CreateService(context);
            WorkLogDto dto = new WorkLogDto { Id = 1, StartTime = DateTime.Now.AddHours(-1), EndTime = DateTime.Now, Status = WorkLogStatus.Finished, Type = WorkLogType.Work, UserId = workLog.UserId };
            WorkLogDto result = await service.UpdateAsync(1, dto);

            Assert.NotNull(result);
            Assert.Equal(WorkLogStatus.Finished, result.Status);
        }
    }

    [Fact]
    public async Task UpdateAsync_ReturnsNull_WhenNotFound()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase("UpdateAsyncNotFoundDb").Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            WorkLogService service = CreateService(context);
            WorkLogDto dto = new WorkLogDto { Id = 1, StartTime = DateTime.Now, Status = WorkLogStatus.Finished, Type = WorkLogType.Work, UserId = Guid.NewGuid() };
            WorkLogDto result = await service.UpdateAsync(1, dto);
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task DeleteAsync_DeletesWorkLog()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase("DeleteAsyncDb").Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            WorkLog workLog = new WorkLog { Id = 1, UserId = Guid.NewGuid(), StartTime = DateTime.Now, Type = WorkLogType.Work, ExistenceStatus = ExistenceStatus.Exist };
            context.WorkLogs.Add(workLog);
            context.SaveChanges();

            WorkLogService service = CreateService(context);
            bool result = await service.DeleteAsync(1);

            Assert.True(result);
            Assert.Equal(ExistenceStatus.Deleted, context.WorkLogs.First().ExistenceStatus);
        }
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenNotFound()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase("DeleteAsyncNotFoundDb").Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            WorkLogService service = CreateService(context);
            bool result = await service.DeleteAsync(123);
            Assert.False(result);
        }
    }

    [Fact]
    public async Task RestoreAsync_RestoresWorkLog()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase("RestoreAsyncDb").Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            WorkLog workLog = new WorkLog { Id = 1, UserId = Guid.NewGuid(), StartTime = DateTime.Now, Type = WorkLogType.Work, ExistenceStatus = ExistenceStatus.Deleted };
            context.WorkLogs.Add(workLog);
            context.SaveChanges();

            WorkLogService service = CreateService(context);
            WorkLogDto result = await service.RestoreAsync(1);

            Assert.NotNull(result);
            Assert.Equal(ExistenceStatus.Exist, context.WorkLogs.First().ExistenceStatus);
        }
    }

    [Fact]
    public async Task RestoreAsync_ReturnsNull_WhenNotFound()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase("RestoreAsyncNotFoundDb").Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            WorkLogService service = CreateService(context);
            WorkLogDto result = await service.RestoreAsync(123);
            Assert.Null(result);
        }
    }

    [Fact]
    public async Task GetUnfinishedUsersAsync_ReturnsUserIds()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase("GetUnfinishedUsersAsyncDb").Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            Guid userId = Guid.NewGuid();
            context.WorkLogs.Add(new WorkLog { UserId = userId, StartTime = DateTime.Now, Type = WorkLogType.Work, ExistenceStatus = ExistenceStatus.Exist, EndTime = null });
            context.SaveChanges();

            WorkLogService service = CreateService(context);
            List<Guid> result = await service.GetUnfinishedUsersAsync();

            Assert.Single(result);
            Assert.Equal(userId, result.First());
        }
    }

    [Fact]
    public async Task GetUsedBreakMinutesTodayAsync_ReturnsMinutes()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase("GetUsedBreakMinutesTodayAsyncDb").Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            Guid userId = Guid.NewGuid();
            DateTime today = DateTime.Today;
            context.WorkLogs.Add(new WorkLog { UserId = userId, StartTime = today.AddHours(10), EndTime = today.AddHours(10).AddMinutes(30), Type = WorkLogType.Break, ExistenceStatus = ExistenceStatus.Exist });
            context.SaveChanges();

            WorkLogService service = CreateService(context);
            int result = await service.GetUsedBreakMinutesTodayAsync(userId);

            Assert.Equal(30, result);
        }
    }

    [Fact]
    public async Task GetUsedWorkMinutesTodayAsync_ReturnsMinutes()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase("GetUsedWorkMinutesTodayAsyncDb").Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            Guid userId = Guid.NewGuid();
            DateTime today = DateTime.Today;
            context.WorkLogs.Add(new WorkLog { UserId = userId, StartTime = today.AddHours(8), EndTime = today.AddHours(12), Type = WorkLogType.Work, ExistenceStatus = ExistenceStatus.Exist });
            context.SaveChanges();

            WorkLogService service = CreateService(context);
            int result = await service.GetUsedWorkMinutesTodayAsync(userId);

            Assert.Equal(240, result);
        }
    }
}