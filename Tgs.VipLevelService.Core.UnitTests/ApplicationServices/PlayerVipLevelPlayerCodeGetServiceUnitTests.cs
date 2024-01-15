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

		var query = new PlayerVipLevelPlayerCodeQuery(
			PlayerCode: "4ucmqplxb029");

		var playerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: query.PlayerCode,
			VipLevelId: Guid.NewGuid(),
			VipLevel: 1,
			AutoState: PlayerVipLevelAutoState.Enable);
		_ = fakePlayerVipLevelRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelInfo);

		var service = new PlayerVipLevelPlayerCodeGetService(
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelFixesService);

		var actual = await service.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(1, actual.VipLevel);
	}

	[Fact]
	public async Task Normal_Process_Fixes()
	{
		var fakePlayerVipLevelRepository = Substitute.For<IPlayerVipLevelRepository>();
		var fakePlayerVipLevelFixesService = Substitute.For<IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>>();

		var query = new PlayerVipLevelPlayerCodeQuery(
			PlayerCode: "4ucmqplxb029");

		_ = fakePlayerVipLevelRepository.GetAsync(
			playerCode: Arg.Is(query.PlayerCode),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns((PlayerVipLevelInfo?)null);

		var playerVipLevelInfo = new PlayerVipLevelInfo(
			PlayerCode: query.PlayerCode,
			VipLevelId: Guid.NewGuid(),
			VipLevel: 1,
			AutoState: PlayerVipLevelAutoState.Enable);
		var fixesQuery = new PlayerVipLevelFixesQuery(
			query.PlayerCode);
		_ = fakePlayerVipLevelFixesService.GetAsync(
			query: Arg.Is(fixesQuery),
			cancellationToken: Arg.Any<CancellationToken>())
			.Returns(playerVipLevelInfo);

		var service = new PlayerVipLevelPlayerCodeGetService(
			fakePlayerVipLevelRepository,
			fakePlayerVipLevelFixesService);

		var actual = await service.GetAsync(query);

		Assert.NotNull(actual);
		Assert.Equal(1, actual.VipLevel);
	}
}
