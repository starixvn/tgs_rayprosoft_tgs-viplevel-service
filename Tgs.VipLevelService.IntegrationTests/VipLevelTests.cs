using ExpectedObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tgs.VipLevelService.Clients;
using Tgs.VipLevelService.Entities;

namespace Tgs.VipLevelService.IntegrationTests;

[TestClass]
public class VipLevelTests
{
	[TestMethod]
	public async Task GetVipLevels()
	{
		await using var web = TestVipLevelApplication.Factory;

		var httpClient = web.CreateClient();

		var sut = new VipLevelServiceClient(httpClient);

		var brandId = Guid.NewGuid();

		var dbContext = web.Services.GetRequiredService<TgsContext>();

		await dbContext.VipLevels.AddRangeAsync(
			new VipLevel
			{
				Id = Guid.NewGuid(),
				BrandId = brandId,
				CreatedByUsername = "system",
				UpdatedByUsername = "system"
			},
			new VipLevel
			{
				Id = Guid.NewGuid(),
				BrandId = brandId,
				CreatedByUsername = "system",
				UpdatedByUsername = "system"
			},
			new VipLevel
			{
				Id = Guid.NewGuid(),
				BrandId = Guid.NewGuid(),
				CreatedByUsername = "system",
				UpdatedByUsername = "system"
			});

		_ = await dbContext.SaveChangesAsync();

		var actual = await sut.GetBrandAllVipLevelsAsync(brandId).ToArrayAsync();

		Assert.AreEqual(2, actual.Length);
	}

	[TestMethod]
	public async Task GetVipLevels_Data_Should_Match()
	{
		await using var web = TestVipLevelApplication.Factory;

		var httpClient = web.CreateClient();

		var sut = new VipLevelServiceClient(httpClient);

		var brandId = Guid.NewGuid();

		var dbContext = web.Services.GetRequiredService<TgsContext>();

		await dbContext.VipLevels.AddRangeAsync(
			new VipLevel
			{
				Id = Guid.NewGuid(),
				BrandId = brandId,
				CreatedByUsername = "system",
				UpdatedByUsername = "system",
				Level = 10,
				LevelUpAccumulateBetAmount = 1M,
				LevelKeepAccumulateDeposit = 2M,
				LevelKeepAccumulateBetAmount = 3M,
				LevelUpAccumulateDeposit = 4M,
				LevelKeepCondition = true,
				BirthReward = 6M,
				PromotionReward = 7M,
				QuarterlyReward = 8M,
				CardGameRebatePercentage = 9M,
				LiveDealerRebatePercentage = 10M,
				LotteryRebatePercentage = 11M,
				SlotRebatePercentage = 12M,
				SportsRebatePercentage = 13M,
				VirtualSportsRebatePercentage = 14M
			});

		_ = await dbContext.SaveChangesAsync();

		var expected = new
		{
			Level = 10,
			LevelUpAccumulateBetAmount = 1M,
			LevelKeepAccumulateDeposit = 2M,
			LevelKeepAccumulateBetAmount = 3M,
			LevelUpAccumulateDeposit = 4M,
			LevelKeepCondition = true,
			BirthBenefit = 6M,
			PromotionBenefit = 7M,
			QuarterlyBenefit = 8M,
			CardGameRebate = 9M,
			LiveDealerRebate = 10M,
			LotteryRebate = 11M,
			SlotRebate = 12M,
			SportsRebate = 13M,
			VirtualSportsRebate = 14M
		}.ToExpectedObject();

		var actual = await sut.GetBrandAllVipLevelsAsync(brandId).ToArrayAsync();
		var actualItem = actual.First();

		expected.ShouldMatch(actualItem);
	}

	[TestMethod]
	public async Task CreateVipLevel()
	{
		await using var web = TestVipLevelApplication.Factory;

		var httpClient = web.CreateClient();

		var sut = new VipLevelServiceClient(httpClient);

		var brandId = Guid.NewGuid();
		var modifier = "tester";

		var dbContext = web.Services.GetRequiredService<TgsContext>();

		var createInfo = new VipLevelCreateInfo(
			2,
			10M,
			20M,
			30M,
			40M,
			true,
			100M,
			50M,
			60M,
			.1M,
			.2M,
			.3M,
			.4M,
			.5M,
			.6M);

		var actual = await sut.CreateVipLevelAsync(brandId, createInfo, modifier);

		var dbRow = await dbContext.VipLevels.FirstAsync();

		Assert.AreEqual(actual, dbRow.Id);
	}

	[TestMethod]
	public async Task UpdateVipLevel()
	{
		await using var web = TestVipLevelApplication.Factory;

		var httpClient = web.CreateClient();

		var sut = new VipLevelServiceClient(httpClient);

		var brandId = Guid.NewGuid();
		var vipId = Guid.NewGuid();
		var modifier = "tester";

		var dbContext = web.Services.GetRequiredService<TgsContext>();

		var dbRow = await dbContext.VipLevels.AddAsync(new VipLevel
		{
			Id = vipId,
			BrandId = brandId,
			Level = 1,
			CreatedByUsername = "system",
			UpdatedByUsername = "system"
		});

		_ = await dbContext.SaveChangesAsync();

		var updateInfo = VipLevelUpdateInfo.Default with
		{
			Level = 100,
			LevelKeepAccumulateBetAmount = 10M,
			LevelKeepAccumulateDeposit = 10M,
			LevelKeepCondition = true,
			LevelUpAccumulateBetAmount = 10M,
			LevelUpAccumulateDeposit = 10M,
			BirthBenefit = 10M,
			PromotionBenefit = 10M,
			QuarterlyBenefit = 10M,
			LiveDealerRebate = 10M,
			SlotRebate = 10M,
			LotteryRebate = 10M,
			CardGameRebate = 10M,
			SportsRebate = 10M,
			VirtualSportsRebate = 10M
		};

		await sut.UpdateVipLevelAsync(vipId, updateInfo, modifier);

		Assert.AreEqual(updateInfo.Level, dbRow.Entity.Level);
	}
}
