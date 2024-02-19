using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;

public class PlayerVipLevelRegisterCommandServiceUnitTests
{
	[Fact]
	public async Task Normal_Process_VipLevel_Id()
	{
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelExpCreateService = Substitute.For<ICommandService<PlayerVipLevelExpCreateCommand>>();
		var fakePlayerVipLevelRegisterLogPublishService = Substitute.For<ICommandService<PlayerVipLevelRegisterLogPublishCommand>>();

		var command = new PlayerVipLevelRegisterCommand(
			PlayerCode: "4ucmqplxb029",
			VipLevelId: Guid.NewGuid(),
			VipLevel: null,
			AutoState: PlayerVipLevelAutoState.Enable);

		var playerInfo = new PlayerInfo(
			Guid.NewGuid(),
			"TGS",
			Guid.NewGuid(),
			"TGS",
			Guid.NewGuid(),
			"GPI",
			"4ucmqplxb029",
			"4ucmqplxb029",
			WalletId: Guid.NewGuid());
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var vipLevelInfo = new VipLevelInfo(
			command.VipLevelId!.Value,
			1,
			playerInfo.BrandId,
			100,
			100,
			0,
			0,
			true,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			-1,
			-1);
		_ = fakeVipLevelRepository.GetAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			id: Arg.Is(command.VipLevelId!.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(vipLevelInfo);

		var beforePlayerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: playerInfo.PlayerCode,
			VipLevelId: Guid.NewGuid(),
			VipLevel: 1,
			AutoState: PlayerVipLevelAutoState.Enable);
		_ = fakePlayerVipLevelRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(beforePlayerVipLevelInfo);

		var service = new PlayerVipLevelRegisterCommandService(
			fakeVipLevelRepository,
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelExpCreateService,
			fakePlayerVipLevelRegisterLogPublishService);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelRepository
			.Received(1)
			.CreateAsync(
			playerCode: Arg.Is(playerInfo.PlayerCode),
			vipLevelId: Arg.Is(vipLevelInfo.Id),
			vipLevel: Arg.Is(vipLevelInfo.Level),
			autoState: Arg.Is(PlayerVipLevelAutoState.Enable),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpCreateService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelExpCreateCommand(
				PlayerCode: playerInfo.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());

		var publishCommand = new PlayerVipLevelRegisterLogPublishCommand(
			BrandId: playerInfo.BrandId,
			PlayerCode: playerInfo.PlayerCode,
			AfterLevel: vipLevelInfo.Level,
			AfterAutoState: PlayerVipLevelAutoState.Enable);
		await fakePlayerVipLevelRegisterLogPublishService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(publishCommand),
			cancellationToken: Arg.Any<CancellationToken>());
	}

	[Fact]
	public async Task Normal_Process_VipLevel()
	{
		var fakeVipLevelRepository = Substitute.For<IVipLevelRepository>();
		var fakePlayerRepository = Substitute.For<IPlayerRepository>();
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelExpCreateService = Substitute.For<ICommandService<PlayerVipLevelExpCreateCommand>>();
		var fakePlayerVipLevelRegisterLogPublishService = Substitute.For<ICommandService<PlayerVipLevelRegisterLogPublishCommand>>();

		var command = new PlayerVipLevelRegisterCommand(
			PlayerCode: "4ucmqplxb029",
			VipLevelId: null,
			VipLevel: 1,
			AutoState: PlayerVipLevelAutoState.Enable);

		var playerInfo = new PlayerInfo(
			Guid.NewGuid(),
			"TGS",
			Guid.NewGuid(),
			"TGS",
			Guid.NewGuid(),
			"GPI",
			"4ucmqplxb029",
			"4ucmqplxb029",
			WalletId: Guid.NewGuid());
		_ = fakePlayerRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerInfo);

		var vipLevelInfo = new VipLevelInfo(
			Guid.NewGuid(),
			command.VipLevel!.Value,
			playerInfo.BrandId,
			100,
			100,
			0,
			0,
			true,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			0.0m,
			-1,
			-1);
		_ = fakeVipLevelRepository.GetAsync(
			brandId: Arg.Is(playerInfo.BrandId),
			level: Arg.Is(command.VipLevel!.Value),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(vipLevelInfo);

		var beforePlayerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: playerInfo.PlayerCode,
			VipLevelId: Guid.NewGuid(),
			VipLevel: 1,
			AutoState: PlayerVipLevelAutoState.Enable);
		_ = fakePlayerVipLevelRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(beforePlayerVipLevelInfo);

		var service = new PlayerVipLevelRegisterCommandService(
			fakeVipLevelRepository,
			fakePlayerRepository,
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelExpCreateService,
			fakePlayerVipLevelRegisterLogPublishService);

		await service.ExecuteAsync(command);

		_ = fakePlayerVipLevelRepository
			.Received(1)
			.CreateAsync(
			playerCode: Arg.Is(playerInfo.PlayerCode),
			vipLevelId: Arg.Is(vipLevelInfo.Id),
			vipLevel: Arg.Is(vipLevelInfo.Level),
			autoState: Arg.Is(PlayerVipLevelAutoState.Enable),
			cancellationToken: Arg.Any<CancellationToken>());

		_ = fakePlayerVipLevelExpCreateService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(new PlayerVipLevelExpCreateCommand(
				PlayerCode: playerInfo.PlayerCode)),
			cancellationToken: Arg.Any<CancellationToken>());

		var publishCommand = new PlayerVipLevelRegisterLogPublishCommand(
			BrandId: playerInfo.BrandId,
			PlayerCode: playerInfo.PlayerCode,
			AfterLevel: vipLevelInfo.Level,
			AfterAutoState: PlayerVipLevelAutoState.Enable);
		await fakePlayerVipLevelRegisterLogPublishService
			.Received(1)
			.ExecuteAsync(
			command: Arg.Is(publishCommand),
			cancellationToken: Arg.Any<CancellationToken>());
	}
}
