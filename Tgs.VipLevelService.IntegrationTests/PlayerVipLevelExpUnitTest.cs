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
	public async Task PlayerVipLevelExp_AddDeposit()
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
	public async Task PlayerVipLevelExp_AddTurnover()
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
}
