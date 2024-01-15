using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.ApplicationServices;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.Core.UnitTests.ApplicationServices;
public class PlayerVipLevelPlayerCodeGetServiceUnitTests
{
	[Fact]
	public async Task Normal_Process()
	{
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();

		var command = new PlayerVipLevelPlayerCodeQuery(
			PlayerCode: "4ucmqplxb029");

		var playerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: command.PlayerCode,
			VipLevelId: Guid.NewGuid(),
			VipLevel: 1,
			AutoState: PlayerVipLevelAutoState.ENABLE);
		_ = fakePlayerVipLevelRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelInfo);

		var service = new PlayerVipLevelPlayerCodeGetService(
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelFixesService);

		var actual = await service.GetAsync(command);

		Assert.NotNull(actual);
		Assert.Equal(1, playerVipLevelInfo.VipLevel);
	}

	[Fact]
	public async Task Normal_Process_Fixes()
	{
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();

		var command = new PlayerVipLevelPlayerCodeQuery(
			PlayerCode: "4ucmqplxb029");

		_ = fakePlayerVipLevelRepository.GetAsync(
			playerCode: Arg.Is(command.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelInfo?)null);

		var playerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: command.PlayerCode,
			VipLevelId: Guid.NewGuid(),
			VipLevel: 1,
			AutoState: PlayerVipLevelAutoState.ENABLE);
		var query = new PlayerVipLevelFixesQuery(
			command.PlayerCode);
		_ = fakePlayerVipLevelFixesService.GetAsync(
			query: Arg.Is(query),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelInfo);

		var service = new PlayerVipLevelPlayerCodeGetService(
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelFixesService);

		var actual = await service.GetAsync(command);

		Assert.NotNull(actual);
		Assert.Equal(1, playerVipLevelInfo.VipLevel);
	}
}
