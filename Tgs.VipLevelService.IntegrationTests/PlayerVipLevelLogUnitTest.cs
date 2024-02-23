using ExpectedObjects;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.VipLevelService.Clients;

namespace Tgs.VipLevelService.IntegrationTests;

public class PlayerVipLevelLogUnitTest
{
	[Fact]
	public async Task PlayerVipLevelLog_Query()
	{
		var playerCode = "4umcdlkltmuc";
		var startOn = new DateTimeOffset(2024, 02, 06, 00, 00, 00, TimeSpan.Zero);
		var endOn = new DateTimeOffset(2024, 02, 07, 00, 00, 00, TimeSpan.Zero);

		var page = 1;
		var rowCount = 20;

		var results = new Models.PlayerVipLevelLogInfo[]
		{
			new(
				Id: Guid.NewGuid(),
				PlayerCode: "4umcdlkltmuc",
				Type: Core.Protos.PlayerVipLevelLogType.Register,
				Before: "{}",
				After: "{\"VipLevel\": 1, \"AutoState\": 0, \"PlayerCode\": \"4umcdlkltmuc\", \"VipLevelId\": \"3a6b6f1d-3094-4b66-3c7a-08d9262ce1a5\"}",
				CreatedOn: DateTimeOffset.Parse("2024-02-21 10:51:03.771 +0800"),
				CreatedUserId: Guid.Empty,
				CreatedUsername: string.Empty,
				Remark: string.Empty),
			new(
				Id: Guid.NewGuid(),
				PlayerCode: "4umcdlkltmuc",
				Type: Core.Protos.PlayerVipLevelLogType.System,
				Before: "{\"VipLevel\": 1, \"AutoState\": 0, \"PlayerCode\": \"4umcdlkltmuc\", \"VipLevelId\": \"3a6b6f1d-3094-4b66-3c7a-08d9262ce1a5\"}",
				After: "{\"VipLevel\": 2, \"AutoState\": 0, \"PlayerCode\": \"4umcdlkltmuc\", \"VipLevelId\": \"a5e5afe0-e87b-483d-98db-71629ba93290\"}",
				CreatedOn: DateTimeOffset.Parse("2024-02-21 10:53:51.579 +0800"),
				CreatedUserId: Guid.Empty,
				CreatedUsername: string.Empty,
				Remark: string.Empty),
			new(
				Id: Guid.NewGuid(),
				PlayerCode: "4umcdlkltmuc",
				Type: Core.Protos.PlayerVipLevelLogType.System,
				Before: "{\"VipLevel\": 2, \"AutoState\": 0, \"PlayerCode\": \"4umcdlkltmuc\", \"VipLevelId\": \"a5e5afe0-e87b-483d-98db-71629ba93290\"}",
				After: "{\"VipLevel\": 3, \"AutoState\": 0, \"PlayerCode\": \"4umcdlkltmuc\", \"VipLevelId\": \"c75e54ad-d575-4ff8-af99-cc419a1dd051\"}",
				CreatedOn: DateTimeOffset.Parse("2024-02-21 10:53:51.703 +0800"),
				CreatedUserId: Guid.Empty,
				CreatedUsername: string.Empty,
				Remark: string.Empty),
			new(
				Id: Guid.NewGuid(),
				PlayerCode: "4umcdlkltmuc",
				Type: Core.Protos.PlayerVipLevelLogType.System,
				Before: "{\"VipLevel\": 3, \"AutoState\": 0, \"PlayerCode\": \"4umcdlkltmuc\", \"VipLevelId\": \"c75e54ad-d575-4ff8-af99-cc419a1dd051\"}",
				After: "{\"VipLevel\": 4, \"AutoState\": 0, \"PlayerCode\": \"4umcdlkltmuc\", \"VipLevelId\": \"9cfb51f5-1b9e-465c-af04-e65709cf2ce3\"}",
				CreatedOn: DateTimeOffset.Parse("2024-02-21 10:53:51.788 +0800"),
				CreatedUserId: Guid.Empty,
				CreatedUsername: string.Empty,
				Remark: string.Empty),
			new(
				Id: Guid.NewGuid(),
				PlayerCode: "4umcdlkltmuc",
				Type: Core.Protos.PlayerVipLevelLogType.Manual,
				Before: "{\"VipLevel\": 4, \"AutoState\": 0, \"PlayerCode\": \"4umcdlkltmuc\", \"VipLevelId\": \"9cfb51f5-1b9e-465c-af04-e65709cf2ce3\"}",
				After: "{\"VipLevel\": 3, \"AutoState\": 0, \"PlayerCode\": \"4umcdlkltmuc\", \"VipLevelId\": \"c75e54ad-d575-4ff8-af99-cc419a1dd051\"}",
				CreatedOn: DateTimeOffset.Parse("2024-02-21 10:59:06.681 +0800"),
				CreatedUserId: Guid.Parse("4e84a597-6077-4198-3d04-08d9262ce1a5"),
				CreatedUsername: "Gordon.hung",
				Remark: string.Empty),
		};

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelLogRepository>();

			_ = fakeRepository.QueryAsync(
				playerCode: Arg.Is(playerCode),
				startOn: Arg.Is(startOn),
				endOn: Arg.Is(endOn),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(results.ToAsyncEnumerable());

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelLogQueryAsync(
			playerCode: playerCode,
			startOn: startOn,
			endOn: endOn,
			page: page,
			rowCount: rowCount)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal(expected: 5, actual.Length);

		var expectedRegister = new Common.ViewModels.PlayerVipLevelLogQueryViewModel(
			Type: Common.PlayerVipLevelLogType.Register,
			BeforeLevel: 0,
			BeforeLevelId: Guid.Empty,
			BeforeAutoState: Common.PlayerVipLevelAutoState.Enable,
			AfterLevel: 1,
			AfterLevelId: Guid.Parse("3a6b6f1d-3094-4b66-3c7a-08d9262ce1a5"),
			AfterAutoState: Common.PlayerVipLevelAutoState.Enable,
			CreatedOn: DateTimeOffset.Parse("2024-02-21 10:51:03.771 +0800"),
			CreatedUserId: Guid.Empty,
			CreatedUsername: string.Empty,
			Remark: string.Empty);

		expectedRegister.ToExpectedObject(ctx => ctx.IgnoreEqualsOverride()).ShouldMatch(actual.ElementAt(0));

		var expectedSystem = new Common.ViewModels.PlayerVipLevelLogQueryViewModel(
			Type: Common.PlayerVipLevelLogType.System,
			BeforeLevel: 1,
			BeforeLevelId: Guid.Parse("3a6b6f1d-3094-4b66-3c7a-08d9262ce1a5"),
			BeforeAutoState: Common.PlayerVipLevelAutoState.Enable,
			AfterLevel: 2,
			AfterLevelId: Guid.Parse("a5e5afe0-e87b-483d-98db-71629ba93290"),
			AfterAutoState: Common.PlayerVipLevelAutoState.Enable,
			CreatedOn: DateTimeOffset.Parse("2024-02-21 10:53:51.579 +0800"),
			CreatedUserId: Guid.Empty,
			CreatedUsername: string.Empty,
			Remark: string.Empty);

		expectedSystem.ToExpectedObject(ctx => ctx.IgnoreEqualsOverride()).ShouldMatch(actual.ElementAt(1));

		var expectedManual = new Common.ViewModels.PlayerVipLevelLogQueryViewModel(
			Type: Common.PlayerVipLevelLogType.Manual,
			BeforeLevel: 4,
			BeforeLevelId: Guid.Parse("9cfb51f5-1b9e-465c-af04-e65709cf2ce3"),
			BeforeAutoState: Common.PlayerVipLevelAutoState.Enable,
			AfterLevel: 3,
			AfterLevelId: Guid.Parse("c75e54ad-d575-4ff8-af99-cc419a1dd051"),
			AfterAutoState: Common.PlayerVipLevelAutoState.Enable,
			CreatedOn: DateTimeOffset.Parse("2024-02-21 10:59:06.681 +0800"),
			CreatedUserId: Guid.Parse("4e84a597-6077-4198-3d04-08d9262ce1a5"),
			CreatedUsername: "Gordon.hung",
			Remark: string.Empty);

		expectedManual.ToExpectedObject(ctx => ctx.IgnoreEqualsOverride()).ShouldMatch(actual.ElementAt(4));
	}

	[Fact]
	public async Task PlayerVipLevelLog_QueryCount()
	{
		var playerCode = "4umcdlkltmuc";
		var startOn = new DateTimeOffset(2024, 02, 06, 00, 00, 00, TimeSpan.Zero);
		var endOn = new DateTimeOffset(2024, 02, 07, 00, 00, 00, TimeSpan.Zero);

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelLogRepository>();

			_ = fakeRepository.QueryCountAsync(
				playerCode: playerCode,
				startOn: startOn,
				endOn: endOn,
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(1);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelLogQueryCountAsync(
			playerCode: playerCode,
			startOn: startOn,
			endOn: endOn);

		Assert.Equal(1, actual);
	}
}
