using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Clients;
using Tgs.VipLevelService.Common.ViewModels;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.IntegrationTests;

public class VipLevelUnitTests
{
	[Fact]
	public async Task VipLevel_Query()
	{
		var brandId = Guid.NewGuid();

		var web = new TestPlayerVipLevelApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeQueryService = Substitute.For<IQueryService<VipLevelQuery, VipLevelInfo>>();

			var query = new VipLevelQuery(
				BrandId: brandId);
			var vipLevelInfos = new VipLevelInfo[]{
				new (
					Id: Guid.NewGuid(),
					Level: 1,
					BrandId: brandId,
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
			_ = fakeQueryService.QueryAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(vipLevelInfos.ToAsyncEnumerable());
			_ = services.AddTransient(_ => fakeQueryService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.VipLevelQueryAsync(
			brandId: brandId)
			.ToArrayAsync();

		Assert.NotEmpty(actual);
	}

	[Fact]
	public async Task VipLevel_Query_Empty()
	{
		var brandId = Guid.NewGuid();

		var web = new TestPlayerVipLevelApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeQueryService = Substitute.For<IQueryService<VipLevelQuery, VipLevelInfo>>();

			var query = new VipLevelQuery(
				BrandId: brandId);
			_ = fakeQueryService.QueryAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(Enumerable.Empty<VipLevelInfo>().ToAsyncEnumerable());
			_ = services.AddTransient(_ => fakeQueryService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.VipLevelQueryAsync(
			brandId: brandId)
			.ToArrayAsync();

		Assert.Empty(actual);
	}

	[Fact]
	public async Task VipLevel_ByLevel_Query()
	{
		var brandId = Guid.NewGuid();
		var level = 1;

		var web = new TestPlayerVipLevelApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeQueryService = Substitute.For<IGetService<VipLevelByLevelQuery, VipLevelInfo?>>();

			var query = new VipLevelByLevelQuery(
				BrandId: brandId,
				Level: level);
			var vipLevelInfo = new VipLevelInfo(
				Id: Guid.NewGuid(),
				Level: level,
				BrandId: brandId,
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
				WithdrawTotalAmountPerDay: -1);
			_ = fakeQueryService.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(vipLevelInfo);
			_ = services.AddTransient(_ => fakeQueryService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.VipLevelGetAsync(
			brandId: brandId,
			level: level);

		Assert.NotNull(actual);
	}

	[Fact]
	public async Task VipLevel_ByLevel_Query_Null()
	{
		var brandId = Guid.NewGuid();
		var level = 1;

		var web = new TestPlayerVipLevelApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeQueryService = Substitute.For<IGetService<VipLevelByLevelQuery, VipLevelInfo?>>();

			var query = new VipLevelByLevelQuery(
				BrandId: brandId,
				Level: level);
			_ = fakeQueryService.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns((VipLevelInfo?)null);
			_ = services.AddTransient(_ => fakeQueryService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.VipLevelGetAsync(
			brandId: brandId,
			level: level);

		Assert.Null(actual);
	}

	[Fact]
	public async Task VipLevel_ById_Query()
	{
		var id = Guid.NewGuid();

		var web = new TestPlayerVipLevelApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeQueryService = Substitute.For<IGetService<VipLevelByIdQuery, VipLevelInfo?>>();

			var query = new VipLevelByIdQuery(
				Id: id);
			var vipLevelInfo = new VipLevelInfo(
				Id: id,
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
				WithdrawTotalAmountPerDay: -1);
			_ = fakeQueryService.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(vipLevelInfo);
			_ = services.AddTransient(_ => fakeQueryService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.VipLevelGetAsync(id: id);

		Assert.NotNull(actual);
	}

	[Fact]
	public async Task VipLevel_ById_Query_Null()
	{
		var id = Guid.NewGuid();

		var web = new TestPlayerVipLevelApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeQueryService = Substitute.For<IGetService<VipLevelByIdQuery, VipLevelInfo?>>();

			var query = new VipLevelByIdQuery(
				Id: id);
			_ = fakeQueryService.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns((VipLevelInfo?)null);
			_ = services.AddTransient(_ => fakeQueryService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.VipLevelGetAsync(id: id);

		Assert.Null(actual);
	}

	[Fact]
	public async Task VipLevel_Create()
	{
		var brandId = Guid.NewGuid();
		var source = new VipLevelCreateViewModel(
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
			WithdrawTotalAmountPerDay: -1M);
		var modifier = "admin.enabled";

		var vipId = Guid.NewGuid();
		var web = new TestPlayerVipLevelApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeQueryService = Substitute.For<IGetService<VipLevelCreateCommand, Guid>>();

			var query = new VipLevelCreateCommand(
				BrandId: brandId,
				CreateModel: new VipLevelCreate(
				source.LevelUpAccumulateBetAmount,
				source.LevelUpAccumulateDeposit,
				source.LevelKeepAccumulateBetAmount,
				source.LevelKeepAccumulateDeposit,
				source.LevelKeepCondition,
				source.BirthBenefit,
				source.PromotionBenefit,
				source.QuarterlyBenefit,
				source.LiveDealerRebate,
				source.SlotRebate,
				source.LotteryRebate,
				source.CardGameRebate,
				source.SportsRebate,
				source.VirtualSportsRebate,
				source.WithdrawTransactionsPerDay,
				source.WithdrawTotalAmountPerDay),
				modifier);
			_ = fakeQueryService.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(vipId);
			_ = services.AddTransient(_ => fakeQueryService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		var actual = await client.VipLevelCreateAsync(
			brandId: brandId,
			source: source,
			modifier: modifier);

		Assert.Equal(vipId, actual);
	}

	[Fact]
	public async Task VipLevel_Update()
	{
		var vipId = Guid.NewGuid();
		var source = new VipLevelUpdateViewModel(
			Level: 1,
			LevelKeepAccumulateBetAmount: 10M,
			LevelKeepAccumulateDeposit: 10M,
			LevelKeepCondition: true,
			LevelUpAccumulateBetAmount: 10M,
			LevelUpAccumulateDeposit: 10M,
			BirthBenefit: 10M,
			PromotionBenefit: 10M,
			QuarterlyBenefit: 10M,
			LiveDealerRebate: 0.01m,
			SlotRebate: 0.01m,
			LotteryRebate: 0.01m,
			CardGameRebate: 0.01m,
			SportsRebate: 0.01m,
			VirtualSportsRebate: 0.01m,
			WithdrawTransactionsPerDay: 3,
			WithdrawTotalAmountPerDay: 3000M);
		var modifier = "admin.enabled";

		var web = new TestPlayerVipLevelApplication(builder => _ = builder.ConfigureServices(services =>
		{
			var fakeGetService = Substitute.For<IGetService<VipLevelUpdateQuery, VipLevelUpdate>>();
			var fakeCommandService = Substitute.For<ICommandService<VipLevelUpdateCommand>>();

			var query = new VipLevelUpdateQuery(
				Id: vipId);
			var update = new VipLevelUpdate(
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
				WithdrawTotalAmountPerDay: -1M);
			_ = fakeGetService.GetAsync(
				query: Arg.Is(query),
				cancellationToken: Arg.Any<CancellationToken>())
			.Returns(update);

			var com = new VipLevelUpdateCommand(
				Id: vipId,
				Update: new VipLevelUpdate(
					Level: 1,
					LevelUpAccumulateBetAmount: 10M,
					LevelUpAccumulateDeposit: 10M,
					LevelKeepAccumulateBetAmount: 10M,
					LevelKeepAccumulateDeposit: 10M,
					LevelKeepCondition: true,
					BirthBenefit: 10M,
					PromotionBenefit: 10M,
					QuarterlyBenefit: 10M,
					LiveDealerRebate: 0.01m,
					SlotRebate: 0.01m,
					LotteryRebate: 0.01m,
					CardGameRebate: 0.01m,
					SportsRebate: 0.01m,
					VirtualSportsRebate: 0.01m,
					WithdrawTransactionsPerDay: 3,
					WithdrawTotalAmountPerDay: 3000M),
				modifier);
			_ = services.AddTransient(_ => fakeGetService);
			_ = services.AddTransient(_ => fakeCommandService);
		}));

		var httpClient = web.CreateDefaultClient();
		var client = new VipLevelServiceClient(httpClient);

		await client.VipLevelUpdateAsync(
			vipId: vipId,
			source: source,
			modifier: modifier);
	}
}
