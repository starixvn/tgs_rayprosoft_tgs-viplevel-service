using System.Text.Json;
using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class PlayerVipLevelLogQueryServiceUnitTests
{
	[Fact]
	public async Task Normal_Process_Register()
	{
		var fakePlayerVipLevelLogRepository = Substitute.For<IPlayerVipLevelLogRepository>();

		var query = new PlayerVipLevelLogQuery(
			PlayerCode: "4ucmqplxb029",
			StartOn: DateTimeOffset.UtcNow.AddMinutes(-10),
			EndOn: DateTimeOffset.UtcNow,
			Page: 1,
			RowCount: 20);

		var afterLevelId = Guid.NewGuid();
		var afterLevel = 1;
		var playerVipLevelLogInfos = new PlayerVipLevelLogInfo[]{
			new (
				Id: Guid.NewGuid(),
				PlayerCode: query.PlayerCode,
				Type: PlayerVipLevelLogType.Register,
				Before: "{}",
				After: JsonSerializer.Serialize(new PlayerVipLevelInfo(
					PlayerCode: query.PlayerCode,
					VipLevelId: afterLevelId,
					VipLevel: afterLevel,
					AutoState: PlayerVipLevelAutoState.ENABLE)),
				CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
				CreatedUserId: Guid.Empty,
				CreatedUsername: string.Empty
				)
		};
		_ = fakePlayerVipLevelLogRepository.QueryAsync(
			playerCode: Arg.Is(query.PlayerCode),
			startOn: Arg.Is(query.StartOn),
			endOn: Arg.Is(query.EndOn),
			offset: Arg.Is(query.Page - 1),
			limit: Arg.Is(query.RowCount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelLogInfos.ToAsyncEnumerable());

		var service = new PlayerVipLevelLogQueryService(
			fakePlayerVipLevelLogRepository);

		var results = await service.QueryAsync(
			query)
			.ToArrayAsync();

		Assert.NotEmpty(results);
		Assert.Equal(PlayerVipLevelLogType.Register, results.First().Type);
		Assert.Equal(Guid.Empty, results.First().BeforeLevelId);
		Assert.Equal(0, results.First().BeforeLevel);
		Assert.Equal(PlayerVipLevelAutoState.ENABLE, results.First().BeforeAutoState);
		Assert.Equal(afterLevel, results.First().AfterLevel);
		Assert.Equal(afterLevelId, results.First().AfterLevelId);
		Assert.Equal(PlayerVipLevelAutoState.ENABLE, results.First().AfterAutoState);
		Assert.Equal(Guid.Empty, results.First().CreatedUserId);
		Assert.Equal(string.Empty, results.First().CreatedUsername);
	}

	[Fact]
	public async Task Normal_Process_System()
	{
		var fakePlayerVipLevelLogRepository = Substitute.For<IPlayerVipLevelLogRepository>();

		var query = new PlayerVipLevelLogQuery(
			PlayerCode: "4ucmqplxb029",
			StartOn: DateTimeOffset.UtcNow.AddMinutes(-10),
			EndOn: DateTimeOffset.UtcNow,
			Page: 1,
			RowCount: 20);

		var beforeLevelId = Guid.NewGuid();
		var beforeLevel = 1;
		var afterLevelId = Guid.NewGuid();
		var afterLevel = 2;
		var playerVipLevelLogInfos = new PlayerVipLevelLogInfo[]{
			new (
				Id: Guid.NewGuid(),
				PlayerCode: query.PlayerCode,
				Type: PlayerVipLevelLogType.System,
				Before: JsonSerializer.Serialize(new PlayerVipLevelInfo(
					PlayerCode: query.PlayerCode,
					VipLevelId: beforeLevelId,
					VipLevel: beforeLevel,
					AutoState: PlayerVipLevelAutoState.ENABLE)),
				After: JsonSerializer.Serialize(new PlayerVipLevelInfo(
					PlayerCode: query.PlayerCode,
					VipLevelId: afterLevelId,
					VipLevel: afterLevel,
					AutoState: PlayerVipLevelAutoState.ENABLE)),
				CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
				CreatedUserId: Guid.Empty,
				CreatedUsername: string.Empty
				)
		};
		_ = fakePlayerVipLevelLogRepository.QueryAsync(
			playerCode: Arg.Is(query.PlayerCode),
			startOn: Arg.Is(query.StartOn),
			endOn: Arg.Is(query.EndOn),
			offset: Arg.Is(query.Page - 1),
			limit: Arg.Is(query.RowCount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelLogInfos.ToAsyncEnumerable());

		var service = new PlayerVipLevelLogQueryService(
			fakePlayerVipLevelLogRepository);

		var results = await service.QueryAsync(
			query)
			.ToArrayAsync();

		Assert.NotEmpty(results);
		Assert.Equal(PlayerVipLevelLogType.System, results.First().Type);
		Assert.Equal(beforeLevelId, results.First().BeforeLevelId);
		Assert.Equal(beforeLevel, results.First().BeforeLevel);
		Assert.Equal(PlayerVipLevelAutoState.ENABLE, results.First().BeforeAutoState);
		Assert.Equal(afterLevel, results.First().AfterLevel);
		Assert.Equal(afterLevelId, results.First().AfterLevelId);
		Assert.Equal(PlayerVipLevelAutoState.ENABLE, results.First().AfterAutoState);
		Assert.Equal(Guid.Empty, results.First().CreatedUserId);
		Assert.Equal(string.Empty, results.First().CreatedUsername);
	}

	[Fact]
	public async Task Normal_Process_Manual()
	{
		var fakePlayerVipLevelLogRepository = Substitute.For<IPlayerVipLevelLogRepository>();

		var query = new PlayerVipLevelLogQuery(
			PlayerCode: "4ucmqplxb029",
			StartOn: DateTimeOffset.UtcNow.AddMinutes(-10),
			EndOn: DateTimeOffset.UtcNow,
			Page: 1,
			RowCount: 20);

		var beforeLevelId = Guid.NewGuid();
		var beforeLevel = 1;
		var afterLevelId = Guid.NewGuid();
		var afterLevel = 2;
		var createdUserId = Guid.NewGuid();
		var createdUsername = "admin.enabled";
		var playerVipLevelLogInfos = new PlayerVipLevelLogInfo[]{
			new (
				Id: Guid.NewGuid(),
				PlayerCode: query.PlayerCode,
				Type: PlayerVipLevelLogType.System,
				Before: JsonSerializer.Serialize(new PlayerVipLevelInfo(
					PlayerCode: query.PlayerCode,
					VipLevelId: beforeLevelId,
					VipLevel: beforeLevel,
					AutoState: PlayerVipLevelAutoState.ENABLE)),
				After: JsonSerializer.Serialize(new PlayerVipLevelInfo(
					PlayerCode: query.PlayerCode,
					VipLevelId: afterLevelId,
					VipLevel: afterLevel,
					AutoState: PlayerVipLevelAutoState.ENABLE)),
				CreatedOn: DateTimeOffset.UtcNow.AddMinutes(-1),
				CreatedUserId: createdUserId,
				CreatedUsername:  createdUsername
				)
		};
		_ = fakePlayerVipLevelLogRepository.QueryAsync(
			playerCode: Arg.Is(query.PlayerCode),
			startOn: Arg.Is(query.StartOn),
			endOn: Arg.Is(query.EndOn),
			offset: Arg.Is(query.Page - 1),
			limit: Arg.Is(query.RowCount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelLogInfos.ToAsyncEnumerable());

		var service = new PlayerVipLevelLogQueryService(
			fakePlayerVipLevelLogRepository);

		var results = await service.QueryAsync(
			query)
			.ToArrayAsync();

		Assert.NotEmpty(results);
		Assert.Equal(PlayerVipLevelLogType.System, results.First().Type);
		Assert.Equal(beforeLevelId, results.First().BeforeLevelId);
		Assert.Equal(beforeLevel, results.First().BeforeLevel);
		Assert.Equal(PlayerVipLevelAutoState.ENABLE, results.First().BeforeAutoState);
		Assert.Equal(afterLevel, results.First().AfterLevel);
		Assert.Equal(afterLevelId, results.First().AfterLevelId);
		Assert.Equal(PlayerVipLevelAutoState.ENABLE, results.First().AfterAutoState);
		Assert.Equal(createdUserId, results.First().CreatedUserId);
		Assert.Equal(createdUsername, results.First().CreatedUsername);
	}
}
