using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TimeRecorderBACKEND.DataBaseContext;
using TimeRecorderBACKEND.Dtos;
using TimeRecorderBACKEND.Enums;
using TimeRecorderBACKEND.Models;
using TimeRecorderBACKEND.Services;
using Xunit;

public class DayOffServiceTests
{
    [Fact]
    public async Task RequestDayOffAsync_CreatesDayOffRequest()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "DayOffServiceTestDb1")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            DayOffService service = new DayOffService(context);
            Guid userId = Guid.NewGuid();

            DayOffRequestDto result = await service.RequestDayOffAsync(userId, DateTime.Today, DateTime.Today.AddDays(1), "urlop");

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
            Assert.Equal(DateTime.Today, result.DateStart);
            Assert.Equal(DateTime.Today.AddDays(1), result.DateEnd);
        }
    }

    [Fact]
    public async Task RequestDayOffAsync_Throws_WhenEndDateBeforeStartDate()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "DayOffServiceTestDb2")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            DayOffService service = new DayOffService(context);
            Guid userId = Guid.NewGuid();

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.RequestDayOffAsync(userId, DateTime.Today, DateTime.Today.AddDays(-1), "urlop"));
        }
    }

    [Fact]
    public async Task GetUserDayOffsAsync_ReturnsRequests()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "DayOffServiceTestDb3")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            Guid userId = Guid.NewGuid();
            context.DayOffRequests.Add(new DayOffRequest
            {
                UserId = userId,
                DateStart = DateTime.Today,
                DateEnd = DateTime.Today.AddDays(1),
                ExistenceStatus = ExistenceStatus.Exist
            });
            context.SaveChanges();

            DayOffService service = new DayOffService(context);

            IEnumerable<TimeRecorderBACKEND.Dtos.DayOffRequestDto> result = await service.GetUserDayOffsAsync(userId);

            Assert.Single(result);
            Assert.Equal(userId, result.First().UserId);
        }
    }

    [Fact]
    public async Task DeleteDayOffRequestAsync_DeletesRequest()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "DayOffServiceTestDb4")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            DayOffRequest request = new DayOffRequest
            {
                Id = 1,
                UserId = Guid.NewGuid(),
                DateStart = DateTime.Today,
                DateEnd = DateTime.Today.AddDays(1),
                ExistenceStatus = ExistenceStatus.Exist
            };
            context.DayOffRequests.Add(request);
            context.SaveChanges();

            DayOffService service = new DayOffService(context);

            await service.DeleteDayOffRequestAsync(1);

            DayOffRequest deleted = context.DayOffRequests.First(x => x.Id == 1);
            Assert.Equal(ExistenceStatus.Deleted, deleted.ExistenceStatus);
        }
    }

    [Fact]
    public async Task GetDayOffRequestByIdAsync_ReturnsRequest()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "DayOffServiceTestDb5")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            Guid userId = Guid.NewGuid();
            context.Users.Add(new User { Id = userId });
            context.DayOffRequests.Add(new DayOffRequest
            {
                Id = 1,
                UserId = userId,
                DateStart = DateTime.Today,
                DateEnd = DateTime.Today.AddDays(1),
                ExistenceStatus = ExistenceStatus.Exist
            });
            context.SaveChanges();
            DayOffRequest doR = context.Find<DayOffRequest>(1);

            DayOffService service = new DayOffService(context);

            DayOffRequestDto result = await service.GetDayOffRequestByIdAsync(1);

            Assert.NotNull(result);
            Assert.Equal(userId, result.UserId);
        }
    }

    [Fact]
    public async Task RestoreDayOffRequestAsync_RestoresRequest()
    {
        DbContextOptions<WorkTimeDbContext> options = new DbContextOptionsBuilder<WorkTimeDbContext>()
            .UseInMemoryDatabase(databaseName: "DayOffServiceTestDb6")
            .Options;
        using (WorkTimeDbContext context = new WorkTimeDbContext(options))
        {
            DayOffRequest request = new DayOffRequest
            {
                Id = 3,
                UserId = Guid.NewGuid(),
                DateStart = DateTime.Today,
                DateEnd = DateTime.Today.AddDays(1),
                ExistenceStatus = ExistenceStatus.Deleted
            };
            context.DayOffRequests.Add(request);
            context.SaveChanges();

            DayOffService service = new DayOffService(context);

            TimeRecorderBACKEND.Dtos.DayOffRequestDto result = await service.RestoreDayOffRequestAsync(3);

            Assert.NotNull(result);
            Assert.Equal(ExistenceStatus.Exist, context.DayOffRequests.First(x => x.Id == 3).ExistenceStatus);
        }
    }
}