using System.Threading;
using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class VipLevelUpdateCommandServiceUnitTest
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();

		var command = new VipLevelUpdateCommand(
			Id: Guid.NewGuid(),
			Update: new VipLevelUpdate(
				Level: 1,
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

		var vipLevelInfo = new VipLevelInfo(
			Id: command.Id,
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
			id: Arg.Is(command.Id))
			.Returns(vipLevelInfo);

		var sut = new VipLevelUpdateCommandService(fakeVipLevelRepository);

		await sut.ExecuteAsync(command);

		_ = fakeVipLevelRepository
			.Received(1)
			.UpdateAsync(
			id: Arg.Is(vipLevelInfo.Id),
			brandId: Arg.Is(vipLevelInfo.BrandId),
			update: Arg.Is(command.Update),
			modifier: Arg.Is(command.Modifier),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
