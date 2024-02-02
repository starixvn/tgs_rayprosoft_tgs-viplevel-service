using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class VipLevelSnapshotQueryVipLevelQueryServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakeVipLevelSnapshotRepository = Substitute.For<IVipLevelSnapshotRepository>();

		var query = new VipLevelSnapshotQueryVipLevelQuery(
			Id: Guid.NewGuid());

		var vipLevelSnapshotInfo = new VipLevelSnapshotInfo(
			Id: query.Id,
			BrandId: Guid.NewGuid(),
			"[{\"Id\": \"840def61-3d0b-424f-85da-cecd74e18de0\", \"Level\": 1, \"BrandId\": \"4e86719f-243f-46f6-b010-5c758b7285af\", \"SlotRebate\": 4.000000, \"BirthBenefit\": 0, \"SportsRebate\": 5.000000, \"LotteryRebate\": 2.000000, \"CardGameRebate\": 3.000000, \"LiveDealerRebate\": 1.000000, \"PromotionBenefit\": 0, \"QuarterlyBenefit\": 0, \"LevelKeepCondition\": false, \"VirtualSportsRebate\": 6.000000, \"LevelUpAccumulateDeposit\": 0, \"WithdrawTotalAmountPerDay\": -1.000000, \"LevelKeepAccumulateDeposit\": 0, \"LevelUpAccumulateBetAmount\": 0, \"WithdrawTransactionsPerDay\": -1, \"LevelKeepAccumulateBetAmount\": 0}]",
			DateTimeOffset.Now,
			DateTimeOffset.Now,
			Guid.NewGuid(),
			"admin.enabled");
		_ = fakeVipLevelSnapshotRepository.GetAsync(
			id: Arg.Is(query.Id),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(vipLevelSnapshotInfo);

		var service = new VipLevelSnapshotQueryVipLevelQueryService(
			fakeVipLevelSnapshotRepository);

		var actual = await service.QueryAsync(query)
			.ToArrayAsync();

		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task This_VipLevel_Snapshot_Does_Not_Exist()
	{
		var fakeVipLevelSnapshotRepository = Substitute.For<IVipLevelSnapshotRepository>();

		var query = new VipLevelSnapshotQueryVipLevelQuery(
			Id: Guid.NewGuid());

		_ = fakeVipLevelSnapshotRepository.GetAsync(
			id: Arg.Is(query.Id),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((VipLevelSnapshotInfo?)null);

		var service = new VipLevelSnapshotQueryVipLevelQueryService(
			fakeVipLevelSnapshotRepository);

		_ = await Assert.ThrowsAsync<NullReferenceException>(() => service.QueryAsync(query).ToArrayAsync().AsTask());
	}
}
