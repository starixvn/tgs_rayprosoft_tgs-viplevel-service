using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Clients;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.IntegrationTests;

public class PlayerVipLevelExpUnitTest
{
	[Fact]
	public async Task PlayerVipLevelExp_Add_Turnover()
	{
		var playerCode = "4ucmqplxb029";
		var gameProviderId = Guid.NewGuid();
		var gameId = 1;
		var walletId = Guid.NewGuid();
		var externalWagerId = "PG-1744653599482446848";
		var wagerId = 1744653777370501121;
		var amount = 1;

		var fakeService = Substitute.For<ICommandService<PlayerVipLevelExpAddTurnoverCommand>>();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelExpAddTurnoverAsync(playerCode: playerCode, gameProviderId: gameProviderId, gameId: gameId, walletId: walletId, externalWagerId: externalWagerId, wagerId: wagerId, amount: amount);

		var command = new PlayerVipLevelExpAddTurnoverCommand(
			PlayerCode: playerCode,
			GameProviderId: gameProviderId,
			GameId: gameId,
			WalletId: walletId,
			ExternalWagerId: externalWagerId,
			WagerId: wagerId,
			Amount: amount);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(command),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task PlayerVipLevelExp_Add_Turnover_Credit()
	{
		var playerCode = "4ucmqplxb029";
		var formId = Guid.NewGuid().ToString();
		var amount = 1;

		var fakeService = Substitute.For<ICommandService<PlayerVipLevelExpAddTurnoverManualCommand>>();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelExpAddTurnoverCreditAsync(playerCode: playerCode, formId: formId, amount: amount);

		var command = new PlayerVipLevelExpAddTurnoverManualCommand(
			PlayerCode: playerCode,
			FormId: formId,
			Sign: 1,
			Amount: amount);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(command),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task PlayerVipLevelExp_Add_Turnover_Debit()
	{
		var playerCode = "4ucmqplxb029";
		var formId = Guid.NewGuid().ToString();
		var amount = 1;

		var fakeService = Substitute.For<ICommandService<PlayerVipLevelExpAddTurnoverManualCommand>>();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelExpAddTurnoverDebitAsync(playerCode: playerCode, formId: formId, amount: amount);

		var command = new PlayerVipLevelExpAddTurnoverManualCommand(
			PlayerCode: playerCode,
			FormId: formId,
			Sign: -1,
			Amount: amount);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(command),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task PlayerVipLevelExp_Add_Deposit()
	{
		var playerCode = "4ucmqplxb029";
		var formId = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
		var amount = 1;

		var fakeService = Substitute.For<ICommandService<PlayerVipLevelExpAddDepositCommand>>();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelExpAddDepositAsync(playerCode: playerCode, formId: formId, amount: amount);

		var command = new PlayerVipLevelExpAddDepositCommand(
			PlayerCode: playerCode,
			FormId: formId,
			Amount: amount);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(command),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task PlayerVipLevelExp_Add_Deposit_Credit()
	{
		var playerCode = "4ucmqplxb029";
		var formId = Guid.NewGuid().ToString();
		var amount = 1;

		var fakeService = Substitute.For<ICommandService<PlayerVipLevelExpAddDepositManualCommand>>();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelExpAddDepositCreditAsync(playerCode: playerCode, formId: formId, amount: amount);

		var command = new PlayerVipLevelExpAddDepositManualCommand(
			PlayerCode: playerCode,
			FormId: formId,
			Sign: 1,
			Amount: amount);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(command),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task PlayerVipLevelExp_Add_Deposit_Debit()
	{
		var playerCode = "4ucmqplxb029";
		var formId = Guid.NewGuid().ToString();
		var amount = 1;

		var fakeService = Substitute.For<ICommandService<PlayerVipLevelExpAddDepositManualCommand>>();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services => _ = services.AddTransient(_ => fakeService)));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.PlayerVipLevelExpAddDepositDebitAsync(playerCode: playerCode, formId: formId, amount: amount);

		var command = new PlayerVipLevelExpAddDepositManualCommand(
			PlayerCode: playerCode,
			FormId: formId,
			Sign: -1,
			Amount: amount);

		_ = fakeService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(command),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
