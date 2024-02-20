using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Clients;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.IntegrationTests;

public class PlayerVipLevelExpFormUnitTest
{
	[Fact]
	public async Task PlayerVipLevelExpForm_Get()
	{
		var id = Guid.NewGuid();

		var result = new PlayerVipLevelExpFormInfo(
			Id: id,
			BrandId: Guid.NewGuid(),
			PlayerCode: "test",
			VipLevel: 1,
			Type: Core.Protos.PlayerVipLevelExpFormType.Deposit,
			AutoState: true,
			State: Core.Protos.PlayerVipLevelExpFormState.Pending,
			Sign: 1,
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
			var fakeRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();

			_ = fakeRepository.GetAsync(
				id: Arg.Is(id),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(result);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelExpFormGetAsync(id: id);

		Assert.NotNull(actual);
		Assert.Equal(result.Id, actual.Id);
		Assert.Equal(result.BrandId, actual.BrandId);
		Assert.Equal(result.PlayerCode, actual.PlayerCode);
		Assert.Equal(result.VipLevel, actual.VipLevel);
		Assert.Equal((int)result.Type, (int)actual.Type);
		Assert.Equal(result.AutoState, actual.AutoState);
		Assert.Equal((int)result.State, (int)actual.State);
		Assert.Equal(result.Sign, actual.Sign);
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
	public async Task PlayerVipLevelExpForm_Get_Is_Null()
	{
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();

			_ = fakeRepository.GetAsync(
				id: Arg.Is(id),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelExpFormInfo?)null);

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelExpFormGetAsync(id: id);

		Assert.Null(actual);
	}

	[Fact]
	public async Task PlayerVipLevelExp_Form_Create_Deposit_Credit()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var playerCode = "test";
		var amount = 1.0m;
		var remark = "remark";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakePlayerService = Substitute.For<IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult>>();
			var fakeFormService = Substitute.For<IGetService<PlayerVipLevelExpFormCreateDepositCommand, Guid>>();

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
				query: Arg.Is(new PlayerVipLevelSingleQuery(playerCode)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevel);

			_ = fakeFormService.GetAsync(
				query: new PlayerVipLevelExpFormCreateDepositCommand(
					BrandId: playerVipLevel.BrandId,
					PlayerCode: playerVipLevel.PlayerCode,
					VipLevel: playerVipLevel.VipLevel,
					Sign: 1,
					Amount: amount,
					Remark: remark,
					OperatorUserId: operatorUserId,
					OperatorUsername: operatorUsername,
					PlayerVipLevel: playerVipLevel!),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

			_ = services.AddTransient(_ => fakePlayerService);
			_ = services.AddTransient(_ => fakeFormService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelExpFormCreateDepositCreditAsync(playerCode, amount, remark, operatorUserId, operatorUsername);

		Assert.Equal(id, actual);
	}

	[Fact]
	public async Task PlayerVipLevelExp_Form_Create_Deposit_Debit()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var playerCode = "test";
		var amount = 1.0m;
		var remark = "remark";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakePlayerService = Substitute.For<IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult>>();
			var fakeFormService = Substitute.For<IGetService<PlayerVipLevelExpFormCreateDepositCommand, Guid>>();

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
				query: Arg.Is(new PlayerVipLevelSingleQuery(playerCode)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevel);

			_ = fakeFormService.GetAsync(
				query: new PlayerVipLevelExpFormCreateDepositCommand(
					BrandId: playerVipLevel.BrandId,
					PlayerCode: playerVipLevel.PlayerCode,
					VipLevel: playerVipLevel.VipLevel,
					Sign: -1,
					Amount: amount,
					Remark: remark,
					OperatorUserId: operatorUserId,
					OperatorUsername: operatorUsername,
					PlayerVipLevel: playerVipLevel!),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

			_ = services.AddTransient(_ => fakePlayerService);
			_ = services.AddTransient(_ => fakeFormService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelExpFormCreateDepositDebitAsync(playerCode, amount, remark, operatorUserId, operatorUsername);

		Assert.Equal(id, actual);
	}

	[Fact]
	public async Task PlayerVipLevelExp_Form_Create_Turnover_Credit()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var playerCode = "test";
		var amount = 1.0m;
		var remark = "remark";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakePlayerService = Substitute.For<IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult>>();
			var fakeFormService = Substitute.For<IGetService<PlayerVipLevelExpFormCreateTurnoverCommand, Guid>>();

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
				query: Arg.Is(new PlayerVipLevelSingleQuery(playerCode)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevel);

			_ = fakeFormService.GetAsync(
				query: new PlayerVipLevelExpFormCreateTurnoverCommand(
					BrandId: playerVipLevel.BrandId,
					PlayerCode: playerVipLevel.PlayerCode,
					VipLevel: playerVipLevel.VipLevel,
					Sign: 1,
					Amount: amount,
					Remark: remark,
					OperatorUserId: operatorUserId,
					OperatorUsername: operatorUsername,
					PlayerVipLevel: playerVipLevel!),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

			_ = services.AddTransient(_ => fakePlayerService);
			_ = services.AddTransient(_ => fakeFormService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelExpFormCreateTurnoverCreditAsync(playerCode, amount, remark, operatorUserId, operatorUsername);

		Assert.Equal(id, actual);
	}

	[Fact]
	public async Task PlayerVipLevelExp_Form_Create_Turnover_Debit()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var playerCode = "test";
		var amount = 1.0m;
		var remark = "remark";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakePlayerService = Substitute.For<IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult>>();
			var fakeFormService = Substitute.For<IGetService<PlayerVipLevelExpFormCreateTurnoverCommand, Guid>>();

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
				query: Arg.Is(new PlayerVipLevelSingleQuery(playerCode)),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevel);

			_ = fakeFormService.GetAsync(
				query: new PlayerVipLevelExpFormCreateTurnoverCommand(
					BrandId: playerVipLevel.BrandId,
					PlayerCode: playerVipLevel.PlayerCode,
					VipLevel: playerVipLevel.VipLevel,
					Sign: -1,
					Amount: amount,
					Remark: remark,
					OperatorUserId: operatorUserId,
					OperatorUsername: operatorUsername,
					PlayerVipLevel: playerVipLevel!),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

			_ = services.AddTransient(_ => fakePlayerService);
			_ = services.AddTransient(_ => fakeFormService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.PlayerVipLevelExpFormCreateTurnoverDebitAsync(playerCode, amount, remark, operatorUserId, operatorUsername);

		Assert.Equal(id, actual);
	}

	[Fact]
	public async Task PlayerVipLevelExpForm_Pending()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();

			_ = fakeRepository.PendingAsync(
				id: Arg.Is(id),
				updatedUserId: Arg.Is(operatorUserId),
				updatedUsername: Arg.Is(operatorUsername),
				cancellationToken: Arg.Any<CancellationToken>());

			_ = services.AddTransient(_ => fakeRepository);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelExpFormPendingAsync(id, operatorUserId, operatorUsername);
	}

	[Fact]
	public async Task PlayerVipLevelExpForm_Reject()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var remark = "test";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeRepository = Substitute.For<IPlayerVipLevelExpFormRepository>();

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

		await client.PlayerVipLevelExpFormRejectAsync(id, remark, operatorUserId, operatorUsername);
	}

	[Fact]
	public async Task PlayerVipLevelExpForm_Approved()
	{
		var operatorUserId = Guid.NewGuid();
		var operatorUsername = "admin.enabled";
		var amount = 99m;
		var remark = "test";
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<ICommandService<PlayerVipLevelExpFormApprovedCommand>>();

			_ = fakeService.ExecuteAsync(
				command: Arg.Is(new PlayerVipLevelExpFormApprovedCommand(
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

		await client.PlayerVipLevelExpFormApprovedAsync(id, amount, remark, operatorUserId, operatorUsername);
	}
}
