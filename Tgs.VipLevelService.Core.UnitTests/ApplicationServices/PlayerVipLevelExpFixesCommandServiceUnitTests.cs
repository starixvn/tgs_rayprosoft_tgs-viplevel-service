using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.Calculate.Clients;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class PlayerVipLevelExpFixesCommandServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpFixesGetService>.Instance;
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakeCalculateClient = Substitute.For<ICalculateClient>();

		var command = new PlayerVipLevelExpFixesQuery(
			PlayerCode: "4ucmqplxb029");

		var playerVipLevelExp = new PlayerVipLevelExpInfo(
			PlayerCode: command.PlayerCode,
			AccumDepositId: Guid.NewGuid(),
			AccumDeposit: 199M,
			AccumTurnoverId: Guid.NewGuid(),
			AccumTurnover: 299M);
		_ = fakePlayerVipLevelExpRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelExp);

		var sut = new PlayerVipLevelExpFixesGetService(
			fakeLog,
			fakePlayerVipLevelExpRepository,
			fakeCalculateClient);

		var actual = await sut.GetAsync(command);

		Assert.Equal(playerVipLevelExp.PlayerCode, actual.PlayerCode);
		Assert.Equal(playerVipLevelExp.AccumDepositId, actual.AccumDepositId);
		Assert.Equal(playerVipLevelExp.AccumDeposit, actual.AccumDeposit);
		Assert.Equal(playerVipLevelExp.AccumTurnoverId, actual.AccumTurnoverId);
		Assert.Equal(playerVipLevelExp.AccumTurnover, actual.AccumTurnover);

		_ = fakePlayerVipLevelExpRepository
			.Received(1)
			.CreateAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());
		_ = fakeCalculateClient
			.Received(1)
			.CreateCalculateAsync(
			calculateId: Arg.Is(playerVipLevelExp.AccumTurnoverId),
			cancellationToken: Arg.Any<CancellationToken>());
		_ = fakeCalculateClient
			.Received(1)
			.ExchangeAsync(
			calculateId: Arg.Is(playerVipLevelExp.AccumTurnoverId),
			change: Arg.Is(playerVipLevelExp.AccumTurnover),
			cancellationToken: Arg.Any<CancellationToken>());
		_ = fakeCalculateClient
			.Received(1)
			.CreateCalculateAsync(
			calculateId: Arg.Is(playerVipLevelExp.AccumDepositId),
			cancellationToken: Arg.Any<CancellationToken>());
		_ = fakeCalculateClient
			.Received(1)
			.ExchangeAsync(
			calculateId: Arg.Is(playerVipLevelExp.AccumDepositId),
			change: Arg.Is(playerVipLevelExp.AccumDeposit),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_player_is_VIP_level_experience_does_not_exist()
	{
		var fakeLog = NullLogger<PlayerVipLevelExpFixesGetService>.Instance;
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakeCalculateClient = Substitute.For<ICalculateClient>();

		var command = new PlayerVipLevelExpFixesQuery(
			PlayerCode: "4ucmqplxb029");

		_ = fakePlayerVipLevelExpRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelExpInfo?)null);

		var sut = new PlayerVipLevelExpFixesGetService(
			fakeLog,
			fakePlayerVipLevelExpRepository,
			fakeCalculateClient);

		_ = await Assert.ThrowsAsync<NullReferenceException>(async () => await sut.GetAsync(command));

		_ = fakePlayerVipLevelExpRepository
			.Received(1)
			.CreateAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());
		_ = fakeCalculateClient
			.Received(0)
			.CreateCalculateAsync(
			calculateId: Arg.Any<Guid>(),
			cancellationToken: Arg.Any<CancellationToken>());
		_ = fakeCalculateClient
			.Received(0)
			.ExchangeAsync(
			calculateId: Arg.Any<Guid>(),
			change: Arg.Any<decimal>(),
			cancellationToken: Arg.Any<CancellationToken>());
		_ = fakeCalculateClient
			.Received(0)
			.CreateCalculateAsync(
			calculateId: Arg.Any<Guid>(),
			cancellationToken: Arg.Any<CancellationToken>());
		_ = fakeCalculateClient
			.Received(0)
			.ExchangeAsync(
			calculateId: Arg.Any<Guid>(),
			change: Arg.Any<decimal>(),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
