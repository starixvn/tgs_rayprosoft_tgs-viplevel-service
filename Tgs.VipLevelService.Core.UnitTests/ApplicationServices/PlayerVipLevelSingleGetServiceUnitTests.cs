using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class PlayerVipLevelSingleGetServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();
		var fakePlayerVipLevelExpFixesService = Substitute.For<IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo>>();

		var query = new PlayerVipLevelSingleQuery(
			PlayerCode: "4ucmqplxb029");

		var playerInfo = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: query.PlayerCode,
			Username: query.PlayerCode,
			WalletId: Guid.NewGuid());
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var playerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: query.PlayerCode,
			VipLevelId: Guid.NewGuid(),
			VipLevel: 1,
			AutoState: PlayerVipLevelAutoState.Enable);
		_ = fakePlayerVipLevelRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelInfo);

		var playerVipLevelExpInfo = new PlayerVipLevelExpInfo(
			PlayerCode: query.PlayerCode,
			AccumDepositId: Guid.NewGuid(),
			AccumDeposit: 100,
			AccumTurnoverId: Guid.NewGuid(),
			AccumTurnover: 200);
		_ = fakePlayerVipLevelExpRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelExpInfo);

		var currentLevel = new VipLevelInfo(
			Id: playerVipLevelInfo.VipLevelId,
			Level: playerVipLevelInfo.VipLevel,
			BrandId: playerInfo.BrandId,
			LevelUpAccumulateBetAmount: 199,
			LevelUpAccumulateDeposit: 99,
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
			Arg.Is(playerInfo.BrandId),
			Arg.Is(playerVipLevelInfo.VipLevel),
			Arg.Any<CancellationToken>())
			.Returns(currentLevel);

		var nextLevel = new VipLevelInfo(
			Id: Guid.NewGuid(),
			Level: 2,
			BrandId: playerInfo.BrandId,
			LevelUpAccumulateBetAmount: 299,
			LevelUpAccumulateDeposit: 199,
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
			Arg.Is(playerInfo.BrandId),
			Arg.Is(playerVipLevelInfo.VipLevel + 1),
			Arg.Any<CancellationToken>())
			.Returns(nextLevel);

		var sut = new PlayerVipLevelSingleGetService(
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelExpRepository,
			fakeVipLevelRepository,
			fakePlayerVipLevelFixesService,
			fakePlayerVipLevelExpFixesService);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(playerInfo.ResellerId, actual.ResellerId);
		Assert.Equal(playerInfo.ResellerCode, actual.ResellerCode);
		Assert.Equal(playerInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(playerInfo.LicenseeCode, actual.LicenseeCode);
		Assert.Equal(playerInfo.BrandId, actual.BrandId);
		Assert.Equal(playerInfo.BrandCode, actual.BrandCode);
		Assert.Equal(playerInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(playerInfo.Username, actual.Username);
		Assert.Equal(playerVipLevelInfo.VipLevelId, actual.VipLevelId);
		Assert.Equal(playerVipLevelInfo.VipLevel, actual.VipLevel);
		Assert.Equal(playerVipLevelInfo.AutoState, actual.AutoState);
		Assert.Equal(playerVipLevelExpInfo.AccumDepositId, actual.AccumDepositId);
		Assert.Equal(playerVipLevelExpInfo.AccumDeposit, actual.AccumDeposit);
		Assert.Equal(playerVipLevelExpInfo.AccumTurnoverId, actual.AccumTurnoverId);
		Assert.Equal(playerVipLevelExpInfo.AccumTurnover, actual.AccumTurnover);
		Assert.Equal(currentLevel.LevelUpAccumulateBetAmount, actual.LevelUpAccumulateBetAmount);
		Assert.Equal(currentLevel.LevelUpAccumulateDeposit, actual.LevelUpAccumulateDeposit);
		Assert.False(actual.FullLevel);
		Assert.Equal(nextLevel.Id, actual.NextVipLevelId);
		Assert.Equal(nextLevel.Level, actual.NextVipLevel);

		_ = fakePlayerVipLevelFixesService
			.Received(0)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpFixesService
			.Received(0)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelExpFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Player_Vip_Level_Fixes()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();
		var fakePlayerVipLevelExpFixesService = Substitute.For<IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo>>();

		var query = new PlayerVipLevelSingleQuery(
			PlayerCode: "4ucmqplxb029");

		var playerInfo = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: query.PlayerCode,
			Username: query.PlayerCode,
			WalletId: Guid.NewGuid());
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		_ = fakePlayerVipLevelRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelInfo?)null);

		var playerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: query.PlayerCode,
			VipLevelId: Guid.NewGuid(),
			VipLevel: 1,
			AutoState: PlayerVipLevelAutoState.Enable);
		_ = fakePlayerVipLevelFixesService.GetAsync(
			query: Arg.Is(new PlayerVipLevelFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelInfo);

		var playerVipLevelExpInfo = new PlayerVipLevelExpInfo(
			PlayerCode: query.PlayerCode,
			AccumDepositId: Guid.NewGuid(),
			AccumDeposit: 100,
			AccumTurnoverId: Guid.NewGuid(),
			AccumTurnover: 200);
		_ = fakePlayerVipLevelExpRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelExpInfo);

		var currentLevel = new VipLevelInfo(
			Id: playerVipLevelInfo.VipLevelId,
			Level: playerVipLevelInfo.VipLevel,
			BrandId: playerInfo.BrandId,
			LevelUpAccumulateBetAmount: 199,
			LevelUpAccumulateDeposit: 99,
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
			Arg.Is(playerInfo.BrandId),
			Arg.Is(playerVipLevelInfo.VipLevel),
			Arg.Any<CancellationToken>())
			.Returns(currentLevel);

		var nextLevel = new VipLevelInfo(
			Id: Guid.NewGuid(),
			Level: 2,
			BrandId: playerInfo.BrandId,
			LevelUpAccumulateBetAmount: 299,
			LevelUpAccumulateDeposit: 199,
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
			Arg.Is(playerInfo.BrandId),
			Arg.Is(playerVipLevelInfo.VipLevel + 1),
			Arg.Any<CancellationToken>())
			.Returns(nextLevel);

		var sut = new PlayerVipLevelSingleGetService(
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelExpRepository,
			fakeVipLevelRepository,
			fakePlayerVipLevelFixesService,
			fakePlayerVipLevelExpFixesService);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(playerInfo.ResellerId, actual.ResellerId);
		Assert.Equal(playerInfo.ResellerCode, actual.ResellerCode);
		Assert.Equal(playerInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(playerInfo.LicenseeCode, actual.LicenseeCode);
		Assert.Equal(playerInfo.BrandId, actual.BrandId);
		Assert.Equal(playerInfo.BrandCode, actual.BrandCode);
		Assert.Equal(playerInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(playerInfo.Username, actual.Username);
		Assert.Equal(playerVipLevelInfo.VipLevelId, actual.VipLevelId);
		Assert.Equal(playerVipLevelInfo.VipLevel, actual.VipLevel);
		Assert.Equal(playerVipLevelInfo.AutoState, actual.AutoState);
		Assert.Equal(playerVipLevelExpInfo.AccumDepositId, actual.AccumDepositId);
		Assert.Equal(playerVipLevelExpInfo.AccumDeposit, actual.AccumDeposit);
		Assert.Equal(playerVipLevelExpInfo.AccumTurnoverId, actual.AccumTurnoverId);
		Assert.Equal(playerVipLevelExpInfo.AccumTurnover, actual.AccumTurnover);
		Assert.Equal(currentLevel.LevelUpAccumulateBetAmount, actual.LevelUpAccumulateBetAmount);
		Assert.Equal(currentLevel.LevelUpAccumulateDeposit, actual.LevelUpAccumulateDeposit);
		Assert.False(actual.FullLevel);
		Assert.Equal(nextLevel.Id, actual.NextVipLevelId);
		Assert.Equal(nextLevel.Level, actual.NextVipLevel);

		_ = fakePlayerVipLevelExpFixesService
			.Received(0)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelExpFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Player_VipLevel_Exp_Fixes()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();
		var fakePlayerVipLevelExpFixesService = Substitute.For<IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo>>();

		var query = new PlayerVipLevelSingleQuery(
			PlayerCode: "4ucmqplxb029");

		var playerInfo = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: query.PlayerCode,
			Username: query.PlayerCode,
			WalletId: Guid.NewGuid());
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var playerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: query.PlayerCode,
			VipLevelId: Guid.NewGuid(),
			VipLevel: 1,
			AutoState: PlayerVipLevelAutoState.Enable);
		_ = fakePlayerVipLevelRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelInfo);

		_ = fakePlayerVipLevelExpRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelExpInfo?)null);

		var playerVipLevelExpInfo = new PlayerVipLevelExpInfo(
			PlayerCode: query.PlayerCode,
			AccumDepositId: Guid.NewGuid(),
			AccumDeposit: 0,
			AccumTurnoverId: Guid.NewGuid(),
			AccumTurnover: 0);
		_ = fakePlayerVipLevelExpFixesService
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelExpFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelExpInfo);

		var currentLevel = new VipLevelInfo(
			Id: playerVipLevelInfo.VipLevelId,
			Level: playerVipLevelInfo.VipLevel,
			BrandId: playerInfo.BrandId,
			LevelUpAccumulateBetAmount: 199,
			LevelUpAccumulateDeposit: 99,
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
			Arg.Is(playerInfo.BrandId),
			Arg.Is(playerVipLevelInfo.VipLevel),
			Arg.Any<CancellationToken>())
			.Returns(currentLevel);

		var nextLevel = new VipLevelInfo(
			Id: Guid.NewGuid(),
			Level: 2,
			BrandId: playerInfo.BrandId,
			LevelUpAccumulateBetAmount: 299,
			LevelUpAccumulateDeposit: 199,
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
			Arg.Is(playerInfo.BrandId),
			Arg.Is(playerVipLevelInfo.VipLevel + 1),
			Arg.Any<CancellationToken>())
			.Returns(nextLevel);

		var sut = new PlayerVipLevelSingleGetService(
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelExpRepository,
			fakeVipLevelRepository,
			fakePlayerVipLevelFixesService,
			fakePlayerVipLevelExpFixesService);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(playerInfo.ResellerId, actual.ResellerId);
		Assert.Equal(playerInfo.ResellerCode, actual.ResellerCode);
		Assert.Equal(playerInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(playerInfo.LicenseeCode, actual.LicenseeCode);
		Assert.Equal(playerInfo.BrandId, actual.BrandId);
		Assert.Equal(playerInfo.BrandCode, actual.BrandCode);
		Assert.Equal(playerInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(playerInfo.Username, actual.Username);
		Assert.Equal(playerVipLevelInfo.VipLevelId, actual.VipLevelId);
		Assert.Equal(playerVipLevelInfo.VipLevel, actual.VipLevel);
		Assert.Equal(playerVipLevelInfo.AutoState, actual.AutoState);
		Assert.Equal(playerVipLevelExpInfo.AccumDepositId, actual.AccumDepositId);
		Assert.Equal(0.0m, actual.AccumDeposit);
		Assert.Equal(playerVipLevelExpInfo.AccumTurnoverId, actual.AccumTurnoverId);
		Assert.Equal(0.0m, actual.AccumTurnover);
		Assert.Equal(currentLevel.LevelUpAccumulateBetAmount, actual.LevelUpAccumulateBetAmount);
		Assert.Equal(currentLevel.LevelUpAccumulateDeposit, actual.LevelUpAccumulateDeposit);
		Assert.False(actual.FullLevel);
		Assert.Equal(nextLevel.Id, actual.NextVipLevelId);
		Assert.Equal(nextLevel.Level, actual.NextVipLevel);

		_ = fakePlayerVipLevelFixesService
			.Received(0)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Player_VipLevel_Full_Level()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();
		var fakePlayerVipLevelExpFixesService = Substitute.For<IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo>>();

		var query = new PlayerVipLevelSingleQuery(
			PlayerCode: "4ucmqplxb029");

		var playerInfo = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: query.PlayerCode,
			Username: query.PlayerCode,
			WalletId: Guid.NewGuid());
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var playerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: query.PlayerCode,
			VipLevelId: Guid.NewGuid(),
			VipLevel: 2,
			AutoState: PlayerVipLevelAutoState.Enable);
		_ = fakePlayerVipLevelRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelInfo);

		var playerVipLevelExpInfo = new PlayerVipLevelExpInfo(
			PlayerCode: query.PlayerCode,
			AccumDepositId: Guid.NewGuid(),
			AccumDeposit: 100,
			AccumTurnoverId: Guid.NewGuid(),
			AccumTurnover: 200);
		_ = fakePlayerVipLevelExpRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelExpInfo);

		var currentLevel = new VipLevelInfo(
			Id: playerVipLevelInfo.VipLevelId,
			Level: playerVipLevelInfo.VipLevel,
			BrandId: playerInfo.BrandId,
			LevelUpAccumulateBetAmount: 199,
			LevelUpAccumulateDeposit: 99,
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
			Arg.Is(playerInfo.BrandId),
			Arg.Is(playerVipLevelInfo.VipLevel),
			Arg.Any<CancellationToken>())
			.Returns(currentLevel);

		_ = fakeVipLevelRepository.GetAsync(
			Arg.Is(playerInfo.BrandId),
			Arg.Is(playerVipLevelInfo.VipLevel + 1),
			Arg.Any<CancellationToken>())
			.Returns((VipLevelInfo?)null);

		var sut = new PlayerVipLevelSingleGetService(
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelExpRepository,
			fakeVipLevelRepository,
			fakePlayerVipLevelFixesService,
			fakePlayerVipLevelExpFixesService);

		var actual = await sut.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(playerInfo.ResellerId, actual.ResellerId);
		Assert.Equal(playerInfo.ResellerCode, actual.ResellerCode);
		Assert.Equal(playerInfo.LicenseeId, actual.LicenseeId);
		Assert.Equal(playerInfo.LicenseeCode, actual.LicenseeCode);
		Assert.Equal(playerInfo.BrandId, actual.BrandId);
		Assert.Equal(playerInfo.BrandCode, actual.BrandCode);
		Assert.Equal(playerInfo.PlayerCode, actual.PlayerCode);
		Assert.Equal(playerInfo.Username, actual.Username);
		Assert.Equal(playerVipLevelInfo.VipLevelId, actual.VipLevelId);
		Assert.Equal(playerVipLevelInfo.VipLevel, actual.VipLevel);
		Assert.Equal(playerVipLevelInfo.AutoState, actual.AutoState);
		Assert.Equal(playerVipLevelExpInfo.AccumDepositId, actual.AccumDepositId);
		Assert.Equal(playerVipLevelExpInfo.AccumDeposit, actual.AccumDeposit);
		Assert.Equal(playerVipLevelExpInfo.AccumTurnoverId, actual.AccumTurnoverId);
		Assert.Equal(playerVipLevelExpInfo.AccumTurnover, actual.AccumTurnover);
		Assert.Equal(currentLevel.LevelUpAccumulateBetAmount, actual.LevelUpAccumulateBetAmount);
		Assert.Equal(currentLevel.LevelUpAccumulateDeposit, actual.LevelUpAccumulateDeposit);
		Assert.True(actual.FullLevel);
		Assert.Equal(currentLevel.Id, actual.NextVipLevelId);
		Assert.Equal(currentLevel.Level, actual.NextVipLevel);

		_ = fakePlayerVipLevelFixesService
			.Received(0)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpFixesService
			.Received(0)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelExpFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task This_brand_VIP_level_does_not_exist()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();
		var fakePlayerVipLevelExpFixesService = Substitute.For<IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo>>();

		var query = new PlayerVipLevelSingleQuery(
			PlayerCode: "4ucmqplxb029");

		var playerInfo = new PlayerInfo(
			ResellerId: Guid.NewGuid(),
			ResellerCode: "Demo",
			LicenseeId: Guid.NewGuid(),
			LicenseeCode: "TGS",
			BrandId: Guid.NewGuid(),
			BrandCode: "GPI",
			PlayerCode: query.PlayerCode,
			Username: query.PlayerCode,
			WalletId: Guid.NewGuid());
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var playerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: query.PlayerCode,
			VipLevelId: Guid.NewGuid(),
			VipLevel: 1,
			AutoState: PlayerVipLevelAutoState.Enable);
		_ = fakePlayerVipLevelRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelInfo);

		var playerVipLevelExpInfo = new PlayerVipLevelExpInfo(
			PlayerCode: query.PlayerCode,
			AccumDepositId: Guid.NewGuid(),
			AccumDeposit: 100,
			AccumTurnoverId: Guid.NewGuid(),
			AccumTurnover: 200);
		_ = fakePlayerVipLevelExpRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelExpInfo);

		_ = fakeVipLevelRepository.GetAsync(
			Arg.Is(playerInfo.BrandId),
			Arg.Is(playerVipLevelInfo.VipLevel),
			Arg.Any<CancellationToken>())
			.Returns((VipLevelInfo?)null);

		var sut = new PlayerVipLevelSingleGetService(
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelExpRepository,
			fakeVipLevelRepository,
			fakePlayerVipLevelFixesService,
			fakePlayerVipLevelExpFixesService);

		_ = await Assert.ThrowsAsync<NullReferenceException>(async () => await sut.GetAsync(query).ConfigureAwait(false));

		_ = fakePlayerVipLevelFixesService
			.Received(0)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpFixesService
			.Received(0)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelExpFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
