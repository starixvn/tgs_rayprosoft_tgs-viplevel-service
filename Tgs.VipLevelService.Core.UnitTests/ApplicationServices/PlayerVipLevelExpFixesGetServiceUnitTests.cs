using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class PlayerVipLevelExpFixesGetServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();

		var query = new PlayerVipLevelExpFixesQuery(
			PlayerCode: "4ucmqplxb029");

		var playerVipLevelExpInfo = new PlayerVipLevelExpInfo(
			PlayerCode: "4ucmqplxb029",
			AccumDepositId: Guid.NewGuid(),
			AccumDeposit: 0.0m,
			AccumTurnoverId: Guid.NewGuid(),
			AccumTurnover: 0.0m);
		_ = fakePlayerVipLevelExpRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelExpInfo);

		var service = new PlayerVipLevelExpFixesGetService(
			fakePlayerVipLevelExpRepository);

		_ = await service.GetAsync(query);

		_ = fakePlayerVipLevelExpRepository
			.Received(1)
			.CreateAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
