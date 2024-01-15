using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class PlayerVipLevelFixesGetServiceUnitTests
{
	[Fact]
	public async Task Normal_Process_VipLevel_Id()
	{
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();

		var query = new PlayerVipLevelFixesQuery(
			PlayerCode: "4ucmqplxb029");

		var playerVipIdInfo = new PlayerVipIdInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "TGS",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: "4ucmqplxb029",
			Username: "4ucmqplxb029",
			VipId: Guid.NewGuid());
		_ = fakePlayerRepository.GetVipIdAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipIdInfo);

		var vipLevelInfo = new VipLevelInfo(
			Id: playerVipIdInfo.VipId,
			Level: 1,
			BrandId: playerVipIdInfo.BrandId,
			LevelUpAccumulateBetAmount: 100,
			LevelUpAccumulateDeposit: 100,
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
			brandId: Arg.Is(playerVipIdInfo.BrandId),
			id: Arg.Is(playerVipIdInfo.VipId),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(vipLevelInfo);

		var beforePlayerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: playerVipIdInfo.PlayerCode,
			VipLevelId: playerVipIdInfo.VipId,
			VipLevel: 1,
			AutoState: PlayerVipLevelAutoState.Enable);
		_ = fakePlayerVipLevelRepository.GetAsync(
			playerCode: Arg.Is(playerVipIdInfo.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(beforePlayerVipLevelInfo);

		var service = new PlayerVipLevelFixesGetService(
			fakeVipLevelRepository,
			fakePlayerRepository,
			fakePlayerVipLevelRepository);

		_ = await service.GetAsync(query);

		_ = fakePlayerVipLevelRepository
			.Received(1)
			.MergeAsync(
			playerCode: Arg.Is(playerVipIdInfo.PlayerCode),
			vipLevelId: Arg.Is(vipLevelInfo.Id),
			vipLevel: Arg.Is(vipLevelInfo.Level),
			autoState: Arg.Is(PlayerVipLevelAutoState.Enable),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
