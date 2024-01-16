using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class VipLevelCreateCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();

		var sut = new VipLevelCreateCommandService(fakeVipLevelRepository);

		var command = new VipLevelCreateCommand(
			BrandId: Guid.NewGuid(),
			CreateModel: new VipLevelCreate(
				LevelUpAccumulateBetAmount: 10M,
				LevelUpAccumulateDeposit: 20M,
				LevelKeepAccumulateBetAmount: 30M,
				LevelKeepAccumulateDeposit: 40M,
				LevelKeepCondition: true,
				BirthBenefit: 100M,
				PromotionBenefit: 50M,
				QuarterlyBenefit: 60M,
				LiveDealerRebate: .1M,
				SlotRebate: .2M,
				LotteryRebate: .3M,
				CardGameRebate: .4M,
				SportsRebate: .5M,
				VirtualSportsRebate: .6M,
				WithdrawTransactionsPerDay: -1,
				WithdrawTotalAmountPerDay: -1M),
			 Modifier: "admin.enabled");

		_ = await sut.GetAsync(command);

		_ = fakeVipLevelRepository
			.Received(1)
			.CreateAsync(
			id: Arg.Any<Guid>(),
			brandId: Arg.Is(command.BrandId),
			create: Arg.Is(command.CreateModel),
			modifier: Arg.Is(command.Modifier),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
