using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class PlayerVipLevelExpGetServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();

		var query = new PlayerVipLevelExpGetQuery(
			PlayerCode: "4ucmqplxb029");

		var playerVipLevelExpInfo = new PlayerVipLevelExpInfo(
			PlayerCode: query.PlayerCode,
			AccumDepositId: Guid.NewGuid(),
			AccumDeposit: 100m,
			AccumTurnoverId: Guid.NewGuid(),
			AccumTurnover: 199m);
		;
		_ = fakePlayerVipLevelExpRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelExpInfo);

		var service = new PlayerVipLevelExpGetService(
			fakePlayerVipLevelExpRepository);

		var actual = await service.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(100m, actual.AccumDeposit);
		Assert.Equal(199m, actual.AccumTurnover);
	}
}
