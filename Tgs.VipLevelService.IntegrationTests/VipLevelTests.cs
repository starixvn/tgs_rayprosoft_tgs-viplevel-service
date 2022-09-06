using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
			Level = 100
		};

		await sut.UpdateVipLevelAsync(vipId, updateInfo, modifier);

		Assert.AreEqual(updateInfo.Level, dbRow.Entity.Level);
	}
}
