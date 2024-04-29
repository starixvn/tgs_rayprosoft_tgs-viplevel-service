using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using Tgs.GameWagers.Client;
using Tgs.GameWagers.Common;
using Tgs.Payment.Clients;
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
		var fakeLog = NullLogger<PlayerVipLevelSingleGetService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
		var fakeGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();

		var query = new PlayerVipLevelSingleQuery(
			PlayerCode: "4ucmqplxb029");

		var playerInfo = new Models.PlayerInfo(
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

		var paymentOverview = new PaymentOverview(
			FirstDepositAmount: 0.0m,
			FirstDepositHandlingFee: 0.0m,
			FirstDepositOn: DateTime.UtcNow,
			FirstWithdrawAmount: 0.0m,
			FirstWithdrawHandlingFee: 0.0m,
			FirstWithdrawOn: DateTime.UtcNow,
			RecordedBy: new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day),
			TotalDepositAmount: 0.0m,
			TotalDepositCount: 0,
			TotalWithdrawAmount: 0.0m,
			TotalWithdrawCount: 0);
		_ = fakePaymentServiceClient.GetPlayerPaymentOverviewAsync(
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(paymentOverview);

		var gameWagerStatPlayerAccumInfo = new GameWagerStatPlayerAccumInfo(
			PlayerCode: query.PlayerCode,
			Count: 0,
			Bet: 0.0m,
			ValidBet: 0.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m);
		_ = fakeGameWagersClient.GameWagerStatGetPlayerAccumAsync(
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(gameWagerStatPlayerAccumInfo);

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
			fakeLog,
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakeVipLevelRepository,
			fakePaymentServiceClient,
			fakeGameWagersClient,
			fakePlayerVipLevelFixesService);

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
		Assert.Equal(paymentOverview.TotalDepositAmount, actual.AccumDeposit);
		Assert.Equal(gameWagerStatPlayerAccumInfo.Turnover, actual.AccumTurnover);
		Assert.Equal(nextLevel.LevelUpAccumulateBetAmount, actual.LevelUpAccumulateBetAmount);
		Assert.Equal(nextLevel.LevelUpAccumulateDeposit, actual.LevelUpAccumulateDeposit);
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
	public async Task Player_Vip_Level_Fixes()
	{
		var fakeLog = NullLogger<PlayerVipLevelSingleGetService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
		var fakeGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();

		var query = new PlayerVipLevelSingleQuery(
			PlayerCode: "4ucmqplxb029");

		var playerInfo = new Models.PlayerInfo(
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

		var paymentOverview = new PaymentOverview(
			FirstDepositAmount: 0.0m,
			FirstDepositHandlingFee: 0.0m,
			FirstDepositOn: DateTime.UtcNow,
			FirstWithdrawAmount: 0.0m,
			FirstWithdrawHandlingFee: 0.0m,
			FirstWithdrawOn: DateTime.UtcNow,
			RecordedBy: new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day),
			TotalDepositAmount: 0.0m,
			TotalDepositCount: 0,
			TotalWithdrawAmount: 0.0m,
			TotalWithdrawCount: 0);
		_ = fakePaymentServiceClient.GetPlayerPaymentOverviewAsync(
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(paymentOverview);

		var gameWagerStatPlayerAccumInfo = new GameWagerStatPlayerAccumInfo(
			PlayerCode: query.PlayerCode,
			Count: 0,
			Bet: 0.0m,
			ValidBet: 0.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m);
		_ = fakeGameWagersClient.GameWagerStatGetPlayerAccumAsync(
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(gameWagerStatPlayerAccumInfo);

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
			fakeLog,
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakeVipLevelRepository,
			fakePaymentServiceClient,
			fakeGameWagersClient,
			fakePlayerVipLevelFixesService);

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
		Assert.Equal(paymentOverview.TotalDepositAmount, actual.AccumDeposit);
		Assert.Equal(gameWagerStatPlayerAccumInfo.Turnover, actual.AccumTurnover);
		Assert.Equal(nextLevel.LevelUpAccumulateBetAmount, actual.LevelUpAccumulateBetAmount);
		Assert.Equal(nextLevel.LevelUpAccumulateDeposit, actual.LevelUpAccumulateDeposit);
		Assert.False(actual.FullLevel);
		Assert.Equal(nextLevel.Id, actual.NextVipLevelId);
		Assert.Equal(nextLevel.Level, actual.NextVipLevel);
	}

	[Fact]
	public async Task Player_VipLevel_Full_Level()
	{
		var fakeLog = NullLogger<PlayerVipLevelSingleGetService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
		var fakeGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();

		var query = new PlayerVipLevelSingleQuery(
			PlayerCode: "4ucmqplxb029");

		var playerInfo = new Models.PlayerInfo(
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

		var paymentOverview = new PaymentOverview(
			FirstDepositAmount: 0.0m,
			FirstDepositHandlingFee: 0.0m,
			FirstDepositOn: DateTime.UtcNow,
			FirstWithdrawAmount: 0.0m,
			FirstWithdrawHandlingFee: 0.0m,
			FirstWithdrawOn: DateTime.UtcNow,
			RecordedBy: new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day),
			TotalDepositAmount: 0.0m,
			TotalDepositCount: 0,
			TotalWithdrawAmount: 0.0m,
			TotalWithdrawCount: 0);
		_ = fakePaymentServiceClient.GetPlayerPaymentOverviewAsync(
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(paymentOverview);

		var gameWagerStatPlayerAccumInfo = new GameWagerStatPlayerAccumInfo(
			PlayerCode: query.PlayerCode,
			Count: 0,
			Bet: 0.0m,
			ValidBet: 0.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m);
		_ = fakeGameWagersClient.GameWagerStatGetPlayerAccumAsync(
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(gameWagerStatPlayerAccumInfo);

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
			fakeLog,
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakeVipLevelRepository,
			fakePaymentServiceClient,
			fakeGameWagersClient,
			fakePlayerVipLevelFixesService);

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
		Assert.Equal(paymentOverview.TotalDepositAmount, actual.AccumDeposit);
		Assert.Equal(gameWagerStatPlayerAccumInfo.Turnover, actual.AccumTurnover);
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
	}

	[Fact]
	public async Task This_brand_VIP_level_does_not_exist()
	{
		var fakeLog = NullLogger<PlayerVipLevelSingleGetService>.Instance;
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePaymentServiceClient = Substitute.For<IPaymentServiceClient>();
		var fakeGameWagersClient = Substitute.For<IGameWagersClient>();
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();

		var query = new PlayerVipLevelSingleQuery(
			PlayerCode: "4ucmqplxb029");

		var playerInfo = new Models.PlayerInfo(
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

		var paymentOverview = new PaymentOverview(
			FirstDepositAmount: 0.0m,
			FirstDepositHandlingFee: 0.0m,
			FirstDepositOn: DateTime.UtcNow,
			FirstWithdrawAmount: 0.0m,
			FirstWithdrawHandlingFee: 0.0m,
			FirstWithdrawOn: DateTime.UtcNow,
			RecordedBy: new DateOnly(DateTime.UtcNow.Year, DateTime.UtcNow.Month, DateTime.UtcNow.Day),
			TotalDepositAmount: 0.0m,
			TotalDepositCount: 0,
			TotalWithdrawAmount: 0.0m,
			TotalWithdrawCount: 0);
		_ = fakePaymentServiceClient.GetPlayerPaymentOverviewAsync(
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(paymentOverview);

		var gameWagerStatPlayerAccumInfo = new GameWagerStatPlayerAccumInfo(
			PlayerCode: query.PlayerCode,
			Count: 0,
			Bet: 0.0m,
			ValidBet: 0.0m,
			Settle: 0.0m,
			Commission: 0.0m,
			Turnover: 0.0m,
			WinLoss: 0.0m);
		_ = fakeGameWagersClient.GameWagerStatGetPlayerAccumAsync(
			Arg.Is(query.PlayerCode),
			Arg.Any<CancellationToken>())
			.Returns(gameWagerStatPlayerAccumInfo);

		_ = fakeVipLevelRepository.GetAsync(
			Arg.Is(playerInfo.BrandId),
			Arg.Is(playerVipLevelInfo.VipLevel),
			Arg.Any<CancellationToken>())
			.Returns((VipLevelInfo?)null);

		var sut = new PlayerVipLevelSingleGetService(
			fakeLog,
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakeVipLevelRepository,
			fakePaymentServiceClient,
			fakeGameWagersClient,
			fakePlayerVipLevelFixesService);

		_ = await Assert.ThrowsAsync<NullReferenceException>(async () => await sut.GetAsync(query).ConfigureAwait(false));

		_ = fakePlayerVipLevelFixesService
			.Received(0)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
