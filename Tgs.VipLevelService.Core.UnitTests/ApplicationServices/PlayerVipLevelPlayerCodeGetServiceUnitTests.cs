using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class PlayerVipLevelPlayerCodeGetServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();
		var fakePlayerVipLevelExpFixesService = Substitute.For<ICommandService<PlayerVipLevelExpFixesCommand>>();

		var query = new PlayerVipLevelPlayerCodeQuery(
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

		var service = new PlayerVipLevelPlayerCodeGetService(
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelExpRepository,
			fakePlayerVipLevelFixesService,
			fakePlayerVipLevelExpFixesService);

		var actual = await service.GetAsync(query);

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

		_ = fakePlayerVipLevelFixesService
			.Received(0)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpFixesService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelExpFixesCommand(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Player_Vip_Level_Fixes()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();
		var fakePlayerVipLevelExpFixesService = Substitute.For<ICommandService<PlayerVipLevelExpFixesCommand>>();

		var query = new PlayerVipLevelPlayerCodeQuery(
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

		var service = new PlayerVipLevelPlayerCodeGetService(
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelExpRepository,
			fakePlayerVipLevelFixesService,
			fakePlayerVipLevelExpFixesService);

		var actual = await service.GetAsync(query);

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

		_ = fakePlayerVipLevelExpFixesService
			.Received(0)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelExpFixesCommand(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Player_VipLevel_Exp_Fixes()
	{
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelExpRepository = Substitute.For<IPlayerVipLevelExpRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();
		var fakePlayerVipLevelExpFixesService = Substitute.For<ICommandService<PlayerVipLevelExpFixesCommand>>();

		var query = new PlayerVipLevelPlayerCodeQuery(
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

		var service = new PlayerVipLevelPlayerCodeGetService(
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelExpRepository,
			fakePlayerVipLevelFixesService,
			fakePlayerVipLevelExpFixesService);

		var actual = await service.GetAsync(query);

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
		Assert.Equal(Guid.Empty, actual.AccumDepositId);
		Assert.Equal(0.0m, actual.AccumDeposit);
		Assert.Equal(Guid.Empty, actual.AccumTurnoverId);
		Assert.Equal(0.0m, actual.AccumTurnover);

		_ = fakePlayerVipLevelFixesService
			.Received(0)
			.GetAsync(
			query: Arg.Is(new PlayerVipLevelFixesQuery(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpFixesService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelExpFixesCommand(
				PlayerCode: query.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
