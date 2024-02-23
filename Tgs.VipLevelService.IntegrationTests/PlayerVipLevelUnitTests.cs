using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Clients;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.IntegrationTests;

public class PlayerVipLevelUnitTests
{
	[Fact]
	public async Task PlayerVipLevel_Get()
	{
		var playerCode = "4ucmqplxb029";

		var result = new PlayerVipLevelSingleResult(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: playerCode,
			Username: "test",
			VipLevelId: Guid.NewGuid(),
			VipLevel: 1,
			AutoState: Core.Protos.PlayerVipLevelAutoState.Enable,
			AccumDeposit: 100,
			AccumTurnover: 200,
			LevelUpAccumulateBetAmount: 299,
			LevelUpAccumulateDeposit: 199,
			FullLevel: false,
			NextVipLevelId: Guid.NewGuid(),
			NextVipLevel: 2);

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
			{
				var fakeService = Substitute.For<IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult>>();

				var query = new PlayerVipLevelSingleQuery(playerCode);

				_ = fakeService.GetAsync(
					query: Arg.Is(query),
					cancellationToken: Arg.Any<CancellationToken>())
				.Returns(result);

				_ = services.AddTransient(_ => fakeService);
			}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelGetAsync(playerCode: playerCode);

		Assert.NotNull(actual);
		Assert.Equal(result.ResellerId, actual.ResellerId);
		Assert.Equal(result.ResellerCode, actual.ResellerCode);
		Assert.Equal(result.LicenseeId, actual.LicenseeId);
		Assert.Equal(result.LicenseeCode, actual.LicenseeCode);
		Assert.Equal(result.BrandId, actual.BrandId);
		Assert.Equal(result.BrandCode, actual.BrandCode);
		Assert.Equal(result.PlayerCode, actual.PlayerCode);
		Assert.Equal(result.Username, actual.Username);
		Assert.Equal(result.VipLevelId, actual.VipLevelId);
		Assert.Equal(result.VipLevel, actual.VipLevel);
		Assert.Equal((int)result.AutoState, (int)actual.AutoState);
		Assert.Equal(result.AccumDeposit, actual.AccumDeposit);
		Assert.Equal(result.AccumTurnover, actual.AccumTurnover);
	}

	[Fact]
	public async Task PlayerVipLevel_Register_VipLevel()
	{
		var playerCode = "4ucmqplxb029";
		var vipLevel = 1;
		var autoState = Common.PlayerVipLevelAutoState.Enable;

		var fakeService = Substitute.For<ICommandService<PlayerVipLevelRegisterCommand>>();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelRegisterAsync(playerCode: playerCode, vipLevel: vipLevel, autoState: autoState);

		var command = new PlayerVipLevelRegisterCommand(
			PlayerCode: playerCode,
			VipLevelId: null,
			VipLevel: 1,
			AutoState: Core.Protos.PlayerVipLevelAutoState.Enable);

		_ = fakeService
		.Received(1)
		.ExecuteAsync(
			command: Arg.Is(command),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task PlayerVipLevel_Register_VipLevelId()
	{
		var playerCode = "4ucmqplxb029";
		int? vipLevel = null;
		var vipLevelId = Guid.NewGuid();
		var autoState = Common.PlayerVipLevelAutoState.Enable;

		var fakeService = Substitute.For<ICommandService<PlayerVipLevelRegisterCommand>>();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelRegisterAsync(playerCode: playerCode, vipLevel: vipLevel, autoState: autoState, vipLevelId: vipLevelId);

		var command = new PlayerVipLevelRegisterCommand(
			PlayerCode: playerCode,
			VipLevelId: vipLevelId,
			VipLevel: vipLevel,
			AutoState: Core.Protos.PlayerVipLevelAutoState.Enable);

		_ = fakeService
		.Received(1)
		.ExecuteAsync(
			command: Arg.Is(command),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task PlayerVipLevel_System_VipLevel()
	{
		var playerCode = "4ucmqplxb029";
		var vipLevel = 1;

		var fakeService = Substitute.For<ICommandService<PlayerVipLevelSystemCommand>>();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelSystemAsync(playerCode: playerCode, vipLevel: vipLevel);

		var command = new PlayerVipLevelSystemCommand(
			PlayerCode: playerCode,
			VipLevelId: null,
			VipLevel: 1);

		_ = fakeService
		.Received(1)
		.ExecuteAsync(
			command: Arg.Is(command),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task PlayerVipLevel_System_VipLevelId()
	{
		var playerCode = "4ucmqplxb029";
		int? vipLevel = null;
		var vipLevelId = Guid.NewGuid();

		var fakeService = Substitute.For<ICommandService<PlayerVipLevelSystemCommand>>();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelSystemAsync(playerCode: playerCode, vipLevel: vipLevel, vipLevelId: vipLevelId);

		var command = new PlayerVipLevelSystemCommand(
			PlayerCode: playerCode,
			VipLevelId: vipLevelId,
			VipLevel: vipLevel);

		_ = fakeService
		.Received(1)
		.ExecuteAsync(
			command: Arg.Is(command),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task PlayerVipLevel_Manual_VipLevel()
	{
		var playerCode = "4ucmqplxb029";
		var vipLevel = 2;
		var autoState = Common.PlayerVipLevelAutoState.Enable;
		var createdUserId = Guid.NewGuid();
		var createdUsername = "admin.enabled";
		var remark = "調整玩家等級2";

		var fakeService = Substitute.For<ICommandService<PlayerVipLevelManualCommand>>();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelManualAsync(playerCode: playerCode, vipLevel: vipLevel, autoState: autoState, modifierUserId: createdUserId, modifierUsername: createdUsername, remark: remark);

		var command = new PlayerVipLevelManualCommand(
			PlayerCode: playerCode,
			VipLevelId: null,
			VipLevel: 2,
			AutoState: Core.Protos.PlayerVipLevelAutoState.Enable,
			CreatedUserId: createdUserId,
			CreatedUsername: createdUsername,
			Remark: remark);

		_ = fakeService
		.Received(1)
		.ExecuteAsync(
			command: Arg.Is(command),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task PlayerVipLevel_Manual_VipLevelId()
	{
		var playerCode = "4ucmqplxb029";
		int? vipLevel = null;
		var vipLevelId = Guid.NewGuid();
		var autoState = Common.PlayerVipLevelAutoState.Enable;
		var createdUserId = Guid.NewGuid();
		var createdUsername = "admin.enabled";
		var remark = "調整玩家等級2";

		var fakeService = Substitute.For<ICommandService<PlayerVipLevelManualCommand>>();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelManualAsync(playerCode: playerCode, vipLevel: vipLevel, autoState: autoState, modifierUserId: createdUserId, modifierUsername: createdUsername, remark: remark, vipLevelId: vipLevelId);

		var command = new PlayerVipLevelManualCommand(
			PlayerCode: playerCode,
			VipLevelId: vipLevelId,
			VipLevel: vipLevel,
			AutoState: Core.Protos.PlayerVipLevelAutoState.Enable,
			CreatedUserId: createdUserId,
			CreatedUsername: createdUsername,
			Remark: remark);

		_ = fakeService
		.Received(1)
		.ExecuteAsync(
			command: Arg.Is(command),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
