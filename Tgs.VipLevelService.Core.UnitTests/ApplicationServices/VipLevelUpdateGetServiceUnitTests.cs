using System.Threading;
using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class VipLevelUpdateGetServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();

		var query = new VipLevelUpdateQuery(
			Id: Guid.NewGuid());

		var vipLevelInfo = new VipLevelInfo(
			Id: query.Id,
			Level: 1,
			BrandId: Guid.NewGuid(),
			LevelUpAccumulateBetAmount: 0,
			LevelUpAccumulateDeposit: 0,
			LevelKeepAccumulateBetAmount: 0,
			LevelKeepAccumulateDeposit: 0,
			LevelKeepCondition: true,
			BirthBenefit: 0.0m,
			PromotionBenefit: 0.0m,
			QuarterlyBenefit: 0.0m,
			LiveDealerRebate: 0.0m,
			SlotRebate: 0.0m,
			LotteryRebate: 0.0m,
			CardGameRebate: 0.0m,
			SportsRebate: 0.0m,
			VirtualSportsRebate: 0.0m,
			WithdrawTransactionsPerDay: -1,
			WithdrawTotalAmountPerDay: -1);
		_ = fakeVipLevelRepository.GetAsync(
			id: Arg.Is(query.Id))
			.Returns(vipLevelInfo);

		var sut = new VipLevelUpdateGetService(fakeVipLevelRepository);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(1, actual.Level);
	}
}
