using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Clients;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.IntegrationTests;

public class PlayerVipLevelUpgradeFormUnitTest
{
	[Fact]
	public async Task PlayerVipLevelUpgradeForm_Get()
	{
		var id = Guid.NewGuid();

		var result = new PlayerVipLevelUpgradeFormInfo(
			Id: id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			VipLevel: 1,
			State: Core.Protos.PlayerVipLevelUpgradeFormState.Pending,
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
			var fakeRepository = Substitute.For<IPlayerVipLevelUpgradeFormRepository>();

			_ = fakeRepository.GetAsync(
				id: Arg.Is(id),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(result);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelUpgradeFormGetAsync(id: id);

		Assert.NotNull(actual);
		Assert.Equal(result.Id, actual.Id);
		Assert.Equal(result.BrandId, actual.BrandId);
		Assert.Equal(result.PlayerCode, actual.PlayerCode);
		Assert.Equal(result.VipLevel, actual.VipLevel);
		Assert.Equal((int)result.State, (int)actual.State);
		Assert.Equal(result.Amount, actual.Amount);
		Assert.Equal(result.ActualAmount, actual.ActualAmount);
		Assert.Equal(result.Remark, actual.Remark);
		Assert.Equal(result.VipLevel, actual.VipLevel);
		Assert.Equal(result.CreatedOn, actual.CreatedOn);
		Assert.Equal(result.CreatedUserId, actual.CreatedUserId);
		Assert.Equal(result.CreatedUsername, actual.CreatedUsername);
		Assert.Equal(result.UpdatedOn, actual.UpdatedOn);
		Assert.Equal(result.UpdatedUserId, actual.UpdatedUserId);
		Assert.Equal(result.UpdatedUsername, actual.UpdatedUsername);
	}

	[Fact]
	public async Task PlayerVipLevelUpgradeForm_Get_Is_Null()
	{
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelUpgradeFormRepository>();

			_ = fakeRepository.GetAsync(
				id: Arg.Is(id),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelUpgradeFormInfo?)null);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelUpgradeFormGetAsync(id: id);

		Assert.Null(actual);
	}

	[Fact]
	public async Task PlayerVipLevelUpgradeForm_Create()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var playerCode = "test";
		var vipLevel = 2;
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakePlayerService = Substitute.For<IGetService<PlayerVipLevelPlayerCodeQuery, PlayerVipLevelPlayerCodeResult>>();
			var fakeSnapshotService = Substitute.For<IGetService<VipLevelSnapshotCurrentSnapshotIdQuery, Guid>>();
			var fakeSnapshotQueryService = Substitute.For<IQueryService<VipLevelSnapshotQueryVipLevelQuery, VipLevelInfo>>();
			var fakeFormService = Substitute.For<IGetService<PlayerVipLevelUpgradeFormCreateCommand, Guid>>();

			var playerVipLevelPlayerCodeResult = new PlayerVipLevelPlayerCodeResult(
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
				AutoState: PlayerVipLevelAutoState.Enable,
				AccumDepositId: Guid.NewGuid(),
				AccumDeposit: 99m,
				AccumTurnoverId: Guid.NewGuid(),
				AccumTurnover: 88m,
				LevelUpAccumulateBetAmount: 299,
				LevelUpAccumulateDeposit: 199,
				FullLevel: false,
				NextVipLevelId: Guid.NewGuid(),
				NextVipLevel: 2);
			_ = fakePlayerService.GetAsync(
				query: Arg.Is(new PlayerVipLevelPlayerCodeQuery(playerCode)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelPlayerCodeResult);

			var snapshotId = Guid.NewGuid();
			_ = fakeSnapshotService.GetAsync(
				query: Arg.Is(new VipLevelSnapshotCurrentSnapshotIdQuery(playerVipLevelPlayerCodeResult.BrandId)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(snapshotId);

			var vipLevelInfos = new VipLevelInfo[]{
				new (
					Id: playerVipLevelPlayerCodeResult.VipLevelId,
					Level: playerVipLevelPlayerCodeResult.VipLevel,
					BrandId: playerVipLevelPlayerCodeResult.BrandId,
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
				query: new PlayerVipLevelUpgradeFormCreateCommand(
					playerVipLevelPlayerCodeResult.BrandId,
					playerVipLevelPlayerCodeResult.PlayerCode,
					playerVipLevelPlayerCodeResult.VipLevel,
					vipLevelInfos.FirstOrDefault()!.PromotionBenefit,
					operatorUserId,
					operatorUsername,
					vipLevelInfos.FirstOrDefault()!),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

			_ = services.AddTransient(_ => fakePlayerService);
			_ = services.AddTransient(_ => fakeSnapshotService);
			_ = services.AddTransient(_ => fakeSnapshotQueryService);
			_ = services.AddTransient(_ => fakeFormService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelUpgradeFormCreateAsync(playerCode, vipLevel, operatorUserId, operatorUsername);

		Assert.Equal(id, actual);
	}

	[Fact]
	public async Task PlayerVipLevelUpgradeForm_Pending()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelUpgradeFormRepository>();

			_ = fakeRepository.PendingAsync(
				id: Arg.Is(id),
				updatedUserId: Arg.Is(operatorUserId),
				updatedUsername: Arg.Is(operatorUsername),
				cancellationToken: Arg.Any<CancellationToken>());

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelUpgradeFormPendingAsync(id, operatorUserId, operatorUsername);
	}

	[Fact]
	public async Task PlayerVipLevelUpgradeForm_Reject()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var remark = "test";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelUpgradeFormRepository>();

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

		await client.PlayerVipLevelUpgradeFormRejectAsync(id, remark, operatorUserId, operatorUsername);
	}

	[Fact]
	public async Task PlayerVipLevelUpgradeForm_Approved()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var amount = 99m;
		var remark = "test";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<ICommandService<PlayerVipLevelUpgradeFormApprovedCommand>>();

			_ = fakeService.ExecuteAsync(
				command: Arg.Is(new PlayerVipLevelUpgradeFormApprovedCommand(
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

		await client.PlayerVipLevelUpgradeFormApprovedAsync(id, amount, remark, operatorUserId, operatorUsername);
	}

	[Fact]
	public async Task PlayerVipLevelUpgradeForm_Exist_Is_True()
	{
		var playerCode = "test";
		var vipLevel = 2;

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelUpgradeFormRepository>();

			_ = fakeRepository.ExistAsync(
				Arg.Is(playerCode),
				Arg.Is(vipLevel),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(true);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actuala = await client.PlayerVipLevelUpgradeFormExistAsync(playerCode, vipLevel);

		Assert.True(actuala);
	}

	[Fact]
	public async Task PlayerVipLevelUpgradeForm_Exist_Is_False()
	{
		var playerCode = "test";
		var vipLevel = 2;

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelUpgradeFormRepository>();

			_ = fakeRepository.ExistAsync(
				Arg.Is(playerCode),
				Arg.Is(vipLevel),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actuala = await client.PlayerVipLevelUpgradeFormExistAsync(playerCode, vipLevel);

		Assert.False(actuala);
	}
}
