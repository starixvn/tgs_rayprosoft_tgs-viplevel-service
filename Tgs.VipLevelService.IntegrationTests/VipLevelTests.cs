using ExpectedObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Tgs.VipLevelService.Clients;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Entities;

namespace Tgs.VipLevelService.IntegrationTests;

public class VipLevelTests
{
	[Fact]
	public async Task Query_Player_Vip_Adjust_Infos()
	{
		await using var web = TestVipLevelApplication.Factory;

		var httpClient = web.CreateClient();

		var sut = new VipLevelServiceClient(httpClient);

		var brandId = Guid.Parse("4e86719f-243f-46f6-b010-5c758b7285af");
		var playerCode = "4ufv37qmtlhj";

		var dbContext = web.Services.GetRequiredService<TgsContext>();

		var vip1LevelId = Guid.Parse("3e369fa5-5453-4583-b2f8-08d920ec59b6");
		var vip2LevelId = Guid.Parse("bcaf95ae-7bbb-4b41-96bb-c96a0867309c");

		await dbContext.VipLevels.AddRangeAsync(
			new VipLevel
			{
				Id = vip1LevelId,
				Level = 1,
				BrandId = brandId,
				CreatedByUsername = "admin.enabled",
				UpdatedByUsername = "admin.enabled"
			},
			new VipLevel
			{
				Id = vip2LevelId,
				Level = 2,
				BrandId = brandId,
				CreatedByUsername = "admin.enabled",
				UpdatedByUsername = "admin.enabled"
			});

		await dbContext.PlayerVipLevelAdjustRecords.AddRangeAsync(
			new PlayerVipLevelAdjustRecord
			{
				PlayerCode = playerCode,
				OldVipId = vip1LevelId,
				NewVipId = vip2LevelId,
				CreatedOn = DateTime.Now.AddDays(-20),
				Remark = "系統晉升/降級",
				Id = Guid.Parse("7f4b749c-41d3-4bd6-bdf8-2d4206786b2d"),
				OperatorId = Guid.Parse("00000000-0000-0000-0000-000000000000")
			},
			new PlayerVipLevelAdjustRecord
			{
				PlayerCode = playerCode,
				OldVipId = vip2LevelId,
				NewVipId = vip1LevelId,
				CreatedOn = DateTime.Now.AddDays(-10),
				Remark = "系統晉升/降級",
				Id = Guid.Parse("1c3cb9a1-8de2-40a4-85e3-7e202cf721fa"),
				OperatorId = Guid.Parse("00000000-0000-0000-0000-000000000000")
			}
			);

		_ = await dbContext.SaveChangesAsync();

		var condition = new Clients.PlayerVipAdjustQueryConditions
		{
			BrandId = brandId,
			PlayerCode = playerCode
		};
		var paging = new Clients.Paging();

		var actual = await sut.QueryPlayerVipAdjustInfosAsync(condition, paging);

		Assert.Equal(2, actual.TotalCount);
	}
}
