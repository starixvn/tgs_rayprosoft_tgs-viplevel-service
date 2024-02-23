using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Clients;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.IntegrationTests;

public class PlayerVipLevelCycleFormUnitTests
{
	[Fact]
	public async Task PlayerVipLevelCycleForm_Get()
	{
		var id = Guid.NewGuid();

		var result = new PlayerVipLevelCycleFormInfo(
			Id: id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			VipLevel: 1,
			CycleType: Core.Protos.PlayerVipLevelCycleFormCycleType.Season,
			CycleOn: DateOnly.Parse("2024-01-01"),
			FromOn: DateOnly.Parse("2023-10-01"),
			ToOn: DateOnly.Parse("2024-01-01"),
			AutoState: true,
			State: Core.Protos.PlayerVipLevelCycleFormState.Pending,
			Amount: 10m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.UtcNow,
			CreatedUserId: Guid.NewGuid(),
			CreatedUsername: "admin.enabled",
			UpdatedOn: DateTimeOffset.UtcNow,
			UpdatedUserId: Guid.NewGuid(),
			UpdatedUsername: "admin");

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();

			_ = fakeRepository.GetAsync(
				id: Arg.Is(id),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(result);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelCycleFormGetAsync(id: id);

		Assert.NotNull(actual);
		Assert.Equal(result.Id, actual.Id);
		Assert.Equal(result.BrandId, actual.BrandId);
		Assert.Equal(result.PlayerCode, actual.PlayerCode);
		Assert.Equal(result.VipLevel, actual.VipLevel);
		Assert.Equal(result.CycleOn, actual.CycleOn);
		Assert.Equal(result.FromOn, actual.FromOn);
		Assert.Equal(result.ToOn, actual.ToOn);
		Assert.Equal(result.AutoState, actual.AutoState);
		Assert.Equal((int)result.State, (int)actual.State);
		Assert.Equal(result.Amount, actual.Amount);
		Assert.Equal(result.ActualAmount, actual.ActualAmount);
		Assert.Equal(result.Remark, actual.Remark);
		Assert.Equal(result.CreatedOn, actual.CreatedOn);
		Assert.Equal(result.CreatedUserId, actual.CreatedUserId);
		Assert.Equal(result.CreatedUsername, actual.CreatedUsername);
		Assert.Equal(result.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(result.UpdatedUserId, actual.UpdatedUserId);
		Assert.Equal(result.UpdatedUsername, actual.UpdatedUsername);
	}

	[Fact]
	public async Task PlayerVipLevelCycleForm_Get_Is_Null()
	{
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();

			_ = fakeRepository.GetAsync(
				id: Arg.Is(id),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelCycleFormInfo?)null);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelCycleFormGetAsync(id: id);

		Assert.Null(actual);
	}

	[Fact]
	public async Task PlayerVipLevelCycleForm_Create()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var playerCode = "test";
		var cycleType = Common.PlayerVipLevelCycleFormCycleType.Season;
		var cycleOn = DateOnly.Parse("2024-01-01");
		var fromOn = DateOnly.Parse("2023-10-01");
		var toOn = DateOnly.Parse("2024-01-01");
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakePlayerService = Substitute.For<IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult>>();
			var fakeSnapshotService = Substitute.For<IGetService<VipLevelSnapshotCurrentSnapshotIdQuery, Guid>>();
			var fakeSnapshotQueryService = Substitute.For<IQueryService<VipLevelSnapshotQueryVipLevelQuery, VipLevelInfo>>();
			var fakeFormService = Substitute.For<IGetService<PlayerVipLevelCycleFormCreateCommand, Guid>>();

			var playerVipLevel = new PlayerVipLevelSingleResult(
				ResellerId: Guid.NewGuid(),
				ResellerCode: "Demo",
				LicenseeId: Guid.NewGuid(),
				LicenseeCode: "TGS",
				BrandId: Guid.NewGuid(),
				BrandCode: "GPI",
				PlayerCode: "4ucmqplxb029",
				Username: "test",
				VipLevelId: Guid.NewGuid(),
				VipLevel: 1,
				AutoState: Core.Protos.PlayerVipLevelAutoState.Enable,
				AccumDeposit: 99m,
				AccumTurnover: 88m,
				LevelUpAccumulateBetAmount: 299,
				LevelUpAccumulateDeposit: 199,
				FullLevel: false,
				NextVipLevelId: Guid.NewGuid(),
				NextVipLevel: 2);
			_ = fakePlayerService.GetAsync(
				query: Arg.Is(new PlayerVipLevelSingleQuery(playerCode)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevel);

			var snapshotId = Guid.NewGuid();
			_ = fakeSnapshotService.GetAsync(
				query: Arg.Is(new VipLevelSnapshotCurrentSnapshotIdQuery(playerVipLevel.BrandId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(snapshotId);

			var vipLevelInfos = new VipLevelInfo[]{
				new (
					Id: playerVipLevel.VipLevelId,
					Level: playerVipLevel.VipLevel,
					BrandId: playerVipLevel.BrandId,
					LevelUpAccumulateBetAmount: 0,
					LevelUpAccumulateDeposit: 0,
					LevelKeepAccumulateBetAmount: 0,
					LevelKeepAccumulateDeposit: 0,
					LevelKeepCondition: true,
					BirthBenefit: 99.0m,
					PromotionBenefit: 100.0m,
					QuarterlyBenefit: 100.0m,
					LiveDealerRebate: 0.0m,
					SlotRebate: 0.0m,
					LotteryRebate: 0.0m,
					CardGameRebate: 0.0m,
					SportsRebate: 0.0m,
					VirtualSportsRebate: 0.0m,
					WithdrawTransactionsPerDay: -1,
					WithdrawTotalAmountPerDay: -1)
			};
			_ = fakeSnapshotQueryService.QueryAsync(
				query: Arg.Is(new VipLevelSnapshotQueryVipLevelQuery(snapshotId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(vipLevelInfos.ToAsyncEnumerable());

			_ = fakeFormService.GetAsync(
				query: new PlayerVipLevelCycleFormCreateCommand(
					BrandId: playerVipLevel.BrandId,
					PlayerCode: playerVipLevel.PlayerCode,
					VipLevel: playerVipLevel.VipLevel,
					CycleType: (Core.Protos.PlayerVipLevelCycleFormCycleType)cycleType,
					CycleOn: cycleOn,
					FromOn: fromOn,
					ToOn: toOn,
					Amount: vipLevelInfos.FirstOrDefault()!.QuarterlyBenefit,
					OperatorUserId: operatorUserId,
					OperatorUsername: operatorUsername,
					VipLevelInfo: vipLevelInfos.FirstOrDefault()!),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

			_ = services.AddTransient(_ => fakePlayerService);
			_ = services.AddTransient(_ => fakeSnapshotService);
			_ = services.AddTransient(_ => fakeSnapshotQueryService);
			_ = services.AddTransient(_ => fakeFormService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelCycleFormCreateAsync(playerCode, cycleType, cycleOn, fromOn, toOn, operatorUserId, operatorUsername);

		Assert.Equal(id, actual);
	}

	[Fact]
	public async Task PlayerVipLevelCycleForm_Pending()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();

			_ = fakeRepository.PendingAsync(
				id: Arg.Is(id),
				updatedUserId: Arg.Is(operatorUserId),
				updatedUsername: Arg.Is(operatorUsername),
				cancellationToken: Arg.Any<CancellationToken>());

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelCycleFormPendingAsync(id, operatorUserId, operatorUsername);
	}

	[Fact]
	public async Task PlayerVipLevelCycleForm_Reject()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var remark = "test";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();

			_ = fakeRepository.RejectAsync(
				id: Arg.Is(id),
				remark: Arg.Is(remark),
				updatedUserId: Arg.Is(operatorUserId),
				updatedUsername: Arg.Is(operatorUsername),
				cancellationToken: Arg.Any<CancellationToken>());

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelCycleFormRejectAsync(id, remark, operatorUserId, operatorUsername);
	}

	[Fact]
	public async Task PlayerVipLevelCycleForm_Approved()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var amount = 99m;
		var remark = "test";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<ICommandService<PlayerVipLevelCycleFormApprovedCommand>>();

			_ = fakeService.ExecuteAsync(
				command: Arg.Is(new PlayerVipLevelCycleFormApprovedCommand(
					Id: id,
					Amount: amount,
					Remark: remark,
					OperatorUserId: operatorUserId,
					OperatorUsername: operatorUsername)),
				cancellationToken: Arg.Any<CancellationToken>());

			_ = services.AddTransient(_ => fakeService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelCycleFormApprovedAsync(id, amount, remark, operatorUserId, operatorUsername);
	}

	[Fact]
	public async Task PlayerVipLevelCycleForm_Exist_Is_True()
	{
		var playerCode = "test";
		var cycleType = Common.PlayerVipLevelCycleFormCycleType.Season;
		var cycleOn = DateOnly.Parse("2024-01-01");

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();

			_ = fakeRepository.ExistAsync(
				Arg.Is(playerCode),
				Arg.Is(Core.Protos.PlayerVipLevelCycleFormCycleType.Season),
				Arg.Is(cycleOn),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actuala = await client.PlayerVipLevelCycleFormExistAsync(playerCode, cycleType, cycleOn);

		Assert.True(actuala);
	}

	[Fact]
	public async Task PlayerVipLevelCycleForm_Exist_Is_False()
	{
		var playerCode = "test";
		var cycleType = Common.PlayerVipLevelCycleFormCycleType.Season;
		var cycleOn = DateOnly.Parse("2024-01-01");

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();

			_ = fakeRepository.ExistAsync(
				Arg.Is(playerCode),
				Arg.Is(Core.Protos.PlayerVipLevelCycleFormCycleType.Season),
				Arg.Is(cycleOn),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actuala = await client.PlayerVipLevelCycleFormExistAsync(playerCode, cycleType, cycleOn);

		Assert.False(actuala);
	}

	[Fact]
	public async Task PlayerVipLevelBirthdayForm_Query()
	{
		var brandId = Guid.NewGuid();
		var updatedOnStart = new DateTimeOffset(2024, 02, 06, 00, 00, 00, TimeSpan.Zero);
		var updatedOnEnd = new DateTimeOffset(2024, 02, 07, 00, 00, 00, TimeSpan.Zero);
		var username = "test";
		var playerCode = "test";
		var states = new Common.PlayerVipLevelCycleFormState[] {
			Common.PlayerVipLevelCycleFormState.Create,
			Common.PlayerVipLevelCycleFormState.Pending
		};
		var page = 1;
		var rowCount = 20;

		var result = new PlayerVipLevelCycleFormQueryInfo(
			Id: Guid.NewGuid(),
			BrandId: Guid.NewGuid(),
			PlayerCode: playerCode,
			Username: username,
			VipLevel: 1,
			CycleType: Core.Protos.PlayerVipLevelCycleFormCycleType.Season,
			CycleOn: DateOnly.Parse("2024-01-01"),
			FromOn: DateOnly.Parse("2023-10-01"),
			ToOn: DateOnly.Parse("2024-01-01"),
			AutoState: true,
			State: Core.Protos.PlayerVipLevelCycleFormState.Pending,
			Amount: 10m,
			ActualAmount: 0.0m,
			Remark: string.Empty,
			CreatedOn: DateTimeOffset.UtcNow,
			CreatedUserId: Guid.NewGuid(),
			CreatedUsername: "admin.enabled",
			UpdatedOn: DateTimeOffset.UtcNow,
			UpdatedUserId: Guid.NewGuid(),
			UpdatedUsername: "admin");

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();

			_ = fakeRepository.QueryAsync(
				brandId: Arg.Is(brandId),
				updatedOnStart: Arg.Is(updatedOnStart),
				updatedOnEnd: Arg.Is(updatedOnEnd),
				username: Arg.Is(username),
				playerCode: Arg.Is(playerCode),
				states: Arg.Any<IEnumerable<Core.Protos.PlayerVipLevelCycleFormState>>(),
				offset: Arg.Is(page - 1),
				limit: Arg.Is(rowCount),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(new PlayerVipLevelCycleFormQueryInfo[] { result }.ToAsyncEnumerable());

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelCycleFormQueryAsync(
			brandId: brandId,
			updatedOnStart: updatedOnStart,
			updatedOnEnd: updatedOnEnd,
			username: username,
			playerCode: playerCode,
			states: states,
			page: page,
			rowCount: rowCount)
			.ToArrayAsync();

		Assert.NotNull(actual);
		Assert.Equal(result.Id, actual.FirstOrDefault()!.Id);
		Assert.Equal(result.BrandId, actual.FirstOrDefault()!.BrandId);
		Assert.Equal(result.PlayerCode, actual.FirstOrDefault()!.PlayerCode);
		Assert.Equal(result.Username, actual.FirstOrDefault()!.Username);
		Assert.Equal(result.VipLevel, actual.FirstOrDefault()!.VipLevel);
		Assert.Equal(result.CycleOn, actual.FirstOrDefault()!.CycleOn);
		Assert.Equal(result.FromOn, actual.FirstOrDefault()!.FromOn);
		Assert.Equal(result.ToOn, actual.FirstOrDefault()!.ToOn);
		Assert.Equal(result.AutoState, actual.FirstOrDefault()!.AutoState);
		Assert.Equal((int)result.State, (int)actual.FirstOrDefault()!.State);
		Assert.Equal(result.Amount, actual.FirstOrDefault()!.Amount);
		Assert.Equal(result.ActualAmount, actual.FirstOrDefault()!.ActualAmount);
		Assert.Equal(result.Remark, actual.FirstOrDefault()!.Remark);
		Assert.Equal(result.CreatedOn, actual.FirstOrDefault()!.CreatedOn);
		Assert.Equal(result.CreatedUserId, actual.FirstOrDefault()!.CreatedUserId);
		Assert.Equal(result.CreatedUsername, actual.FirstOrDefault()!.CreatedUsername);
		Assert.Equal(result.UpdatedOn, actual.FirstOrDefault()!.UpdatedOn);
		Assert.Equal(result.UpdatedUserId, actual.FirstOrDefault()!.UpdatedUserId);
		Assert.Equal(result.UpdatedUsername, actual.FirstOrDefault()!.UpdatedUsername);
	}

	[Fact]
	public async Task PlayerVipLevelBirthdayForm_QueryCount()
	{
		var brandId = Guid.NewGuid();
		var updatedOnStart = new DateTimeOffset(2024, 02, 06, 00, 00, 00, TimeSpan.Zero);
		var updatedOnEnd = new DateTimeOffset(2024, 02, 07, 00, 00, 00, TimeSpan.Zero);
		var username = "test";
		var playerCode = "test";
		var states = new Common.PlayerVipLevelCycleFormState[] {
			Common.PlayerVipLevelCycleFormState.Create,
			Common.PlayerVipLevelCycleFormState.Pending
		};

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelCycleFormRepository>();

			_ = fakeRepository.QueryCountAsync(
				brandId: Arg.Is(brandId),
				updatedOnStart: Arg.Is(updatedOnStart),
				updatedOnEnd: Arg.Is(updatedOnEnd),
				username: Arg.Is(username),
				playerCode: Arg.Is(playerCode),
				states: Arg.Any<IEnumerable<Core.Protos.PlayerVipLevelCycleFormState>>(),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(1);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelCycleFormQueryCountAsync(
			brandId: brandId,
			updatedOnStart: updatedOnStart,
			updatedOnEnd: updatedOnEnd,
			username: username,
			playerCode: playerCode,
			states: states);

		Assert.Equal(1, actual);
	}
}
