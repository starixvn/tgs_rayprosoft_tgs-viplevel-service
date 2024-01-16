using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests;

public class VipLevelTests
{
	[Fact]
	public async Task Query_Player_Vip_Adjust_Infos()
	{
		var fakeTgsRepository = Substitute.For<ITgsRepository>();

		var sut = new PlayerVipAdjustQueryService(fakeTgsRepository);

		var query = new PlayerVipAdjustQuery(
			PlayerCode: "playerCode",
			CreateOnRange: (DateTimeOffset.Now.AddDays(-30), DateTimeOffset.Now),
			   new Paging.Paging<PlayerVipAdjustInfo>());

		_ = await sut.QueryAsync(query).ToArrayAsync();

		_ = fakeTgsRepository.Received(1)
			.QueryPlayerVipAdjustInfosAsync(
				Arg.Any<PlayerVipAdjustQueryConditions>(),
				Arg.Any<Paging.Paging<PlayerVipAdjustInfo>>());
	}

	[Fact]
	public async Task Count_Player_Vip_Adjust_Infos()
	{
		var fakeTgsRepository = Substitute.For<ITgsRepository>();

		var sut = new PlayerVipAdjustQueryService(fakeTgsRepository);

		var query = new PlayerVipAdjustQuery(
			PlayerCode: "playerCode",
			CreateOnRange: (DateTimeOffset.Now.AddDays(-30), DateTimeOffset.Now),
			   new Paging.Paging<PlayerVipAdjustInfo>());

		_ = await sut.GetAsync(query);

		_ = fakeTgsRepository.Received(1)
			.CountPlayerVipAdjustInfosAsync(Arg.Any<PlayerVipAdjustQueryConditions>());
	}
}
