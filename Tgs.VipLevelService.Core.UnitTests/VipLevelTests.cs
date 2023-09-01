using NSubstitute;
using Tgs.VipLevelService.ApplicationServices;

namespace Tgs.VipLevelService.Core.UnitTests;

[TestClass]
public class VipLevelTests
{
	[TestMethod]
	public async Task QueryVipLevels()
	{
		var fakeTgsRepository = Substitute.For<ITgsRepository>();

		var sut = new AllVipLevelsQueryService(fakeTgsRepository);

		var query = new AllVipLevelsQuery(Guid.NewGuid());

		_ = await sut.QueryAsync(query).ToArrayAsync();

		_ = fakeTgsRepository.Received(1)
			.QueryAllVipLevelsAsync(Arg.Is(query.BrandId));
	}

	[TestMethod]
	public async Task Get_VipLevel_BaseModifyModel()
	{
		var fakeTgsRepository = Substitute.For<ITgsRepository>();

		var sut = new GetVipLevelBaseModifyModelQueryService(fakeTgsRepository);

		var query = new GetVipLevelBaseModifyModelQuery(Guid.NewGuid());

		_ = await sut.GetAsync(query);

		_ = fakeTgsRepository.Received(1)
			.GetVipLevelBaseModifyModelAsync(Arg.Is(query.Id));
	}

	[TestMethod]
	public async Task Patch_VipLevel()
	{
		var fakeTgsRepository = Substitute.For<ITgsRepository>();

		var sut = new PatchVipLevelCommandService(fakeTgsRepository);

		var command = new PatchVipLevelCommand(
			Guid.NewGuid(),
			new VipLevelModifyModel(
				1,
				10M,
				20M,
				30M,
				1M,
				true,
				2M,
				3M,
				4M,
				5M,
				6M,
				7M,
				8M,
				9M,
				10M),
			"tester");

		await sut.ExecuteAsync(command);

		_ = fakeTgsRepository.Received(1)
			.ModifyVipLevelAsync(
				Arg.Is(command.Id),
				Arg.Is(command.ModifyModel),
				Arg.Is(command.Modifier));
	}

	[TestMethod]
	public async Task Create_VipLevel()
	{
		var fakeTgsRepository = Substitute.For<ITgsRepository>();

		var sut = new CreateVipLevelCommandService(fakeTgsRepository);

		var command = new CreateVipLevelCommand(
			Guid.NewGuid(),
			new VipLevelCreateModel(
				10M,
				20M,
				30M,
				1M,
				true,
				2M,
				3M,
				4M,
				5M,
				6M,
				7M,
				8M,
				9M,
				10M,
				3,
				5000M),
			  "tester");

		_ = await sut.GetAsync(command);

		_ = fakeTgsRepository.Received(1)
			.CreateVipLevelAsync(
				Arg.Any<Guid>(),
				Arg.Is(command.BrandId),
				Arg.Is(command.CreateModel),
				Arg.Is(command.Modifier));
	}

	[TestMethod]
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

	[TestMethod]
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
