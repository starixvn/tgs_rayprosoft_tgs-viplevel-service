using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Calculate.Clients;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class PlayerVipLevelExpAddTurnoverCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpAddTurnoverCommandService>.Instance;
		var fakeCryptographyUtensil = Substitute.For<ICryptographyUtensil>();
		var fakeCalculateClient = Substitute.For<ICalculateClient>();
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakePlayerVipLevelExpTurnoverRecordRepository = Substitute.For<IPlayerVipLevelExpTurnoverRecordRepository>();
		var fakePlayerVipLevelExpFixesService = Substitute.For<IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo>>();

		var command = new PlayerVipLevelExpAddTurnoverCommand(
			PlayerCode: "4ucmqplxb029",
			GameProviderId: Guid.Parse("6214260a-cf3c-4fb5-ba78-9214fe5608bf"),
			GameId: 3500042,
			WalletId: Guid.Parse("36a6cdef-094f-eb11-8d88-70b5e82b6b8d"),
			ExternalWagerId: "PG-1744653599482446848",
			WagerId: 1744653777370501121,
			Amount: 0.5m);

		var playerVipLevelExpInfo = new PlayerVipLevelExpInfo(
			PlayerCode: command.PlayerCode,
			AccumDepositId: Guid.NewGuid(),
			AccumDeposit: 100,
			AccumTurnoverId: Guid.NewGuid(),
			AccumTurnover: 200);
		_ = fakePlayerVipLevelExpRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelExpInfo);

		var data = string.Concat(
			"GameProviderId", "=", command.GameProviderId, "&",
			"GameId", "=", command.GameId, "&",
			"WalletId", "=", command.WalletId, "&",
			"ExternalWagerId", "=", command.ExternalWagerId);
		var hash = Guid.NewGuid().ToString("N");
		_ = fakeCryptographyUtensil.Sha512(
			source: Arg.Is(data))
			.Returns(hash);

		_ = fakePlayerVipLevelExpTurnoverRecordRepository.ExistAsync(
			Arg.Is(hash),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(false);

		var exchange = new ExchangeResult(playerVipLevelExpInfo.AccumTurnoverId, 200.5m);
		_ = fakeCalculateClient.ExchangeAsync(
			calculateId: Arg.Is(playerVipLevelExpInfo.AccumTurnoverId),
			change: Arg.Is(command.Amount),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(exchange);

		var sut = new PlayerVipLevelExpAddTurnoverCommandService(
			fakeLog,
			fakeCryptographyUtensil,
			fakeCalculateClient,
			fakePlayerVipLevelExpRepository,
			fakePlayerVipLevelExpTurnoverRecordRepository,
			fakePlayerVipLevelExpFixesService);

		await sut.ExecuteAsync(command);

		_ = fakePlayerVipLevelExpRepository
			.Received(1)
			.UpdateTurnoverAsync(
			playerCode: Arg.Is(playerVipLevelExpInfo.PlayerCode),
			accumTurnover: Arg.Is(exchange.PostBalance),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpTurnoverRecordRepository
			.Received(1)
			.AddAsync(
			 hash: Arg.Is(hash),
			 playerCode: Arg.Is(command.PlayerCode),
			 type: Arg.Is(PlayerVipLevelExpTurnoverRecordType.Normal),
			 sign: Arg.Is(1),
			 amount: Arg.Is(command.Amount),
			 balance: Arg.Any<decimal>(),
			 additionalData: Arg.Any<JsonNode>(),
			 remark: Arg.Any<string>(),
			 createdOn: Arg.Any<DateTimeOffset>(),
			 createdUserId: Arg.Any<Guid>(),
			 createdUsername: Arg.Any<string>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
