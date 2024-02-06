using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Clients;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.IntegrationTests;

public class VipLevelSnapshotUnitTests
{
	[Fact]
	public async Task VipLevelSnapshot_Query_VipLevel()
	{
		var id = Guid.NewGuid();

		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IQueryService<VipLevelSnapshotQueryVipLevelQuery, VipLevelInfo>>();

			var query = new VipLevelSnapshotQueryVipLevelQuery(id);
			var vipLevelInfos = new VipLevelInfo[]{
				new (
					Id: Guid.NewGuid(),
					Level: 1,
					BrandId: Guid.NewGuid(),
					LevelUpAccumulateBetAmount: 0,
					LevelUpAccumulateDeposit: 0,
					LevelKeepAccumulateBetAmount: 0,
					LevelKeepAccumulateDeposit: 0,
					LevelKeepCondition: true,
					BirthBenefit: 100.0m,
					PromotionBenefit: 100.0m,
					QuarterlyBenefit: 100.0m,
					LiveDealerRebate: 0.0m,
					SlotRebate: 0.0m,
					LotteryRebate: 0.0m,
					CardGameRebate: 0.0m,
					SportsRebate: 0.0m,
					VirtualSportsRebate: 0.0m,
					WithdrawTransactionsPerDay: -1,
					WithdrawTotalAmountPerDay: -1)
			};
			_ = fakeService.QueryAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(vipLevelInfos.ToAsyncEnumerable());

			_ = services.AddTransient(_ => fakeService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.VipLevelSnapshotQueryVipLevelAsync(
			id: id).ToArrayAsync();

		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task VipLevelSnapshot_Current_SnapshotId()
	{
		var brandId = Guid.NewGuid();
		var id = Guid.NewGuid();
		var web = new VipLevelApplicationTest(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeService = Substitute.For<IGetService<VipLevelSnapshotCurrentSnapshotIdQuery, Guid>>();

			var query = new VipLevelSnapshotCurrentSnapshotIdQuery(brandId);

			_ = fakeService.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(id);

			_ = services.AddTransient(_ => fakeService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.VipLevelSnapshotCurrentSnapshotIdAsync(brandId: brandId);
		Assert.Equal(id, actual);
	}
}
