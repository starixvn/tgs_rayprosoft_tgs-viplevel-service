using Microsoft.Extensions.Logging;
using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelManualCommandService(
	IVipLevelRepository vipLevelRepository,
	IPlayerRepository playerRepository,
	IPlayerVipLevelRepository playerVipLevelRepository,
	IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo> fixesService,
	ICommandService<PlayerVipLevelManualLogPublishCommand> publishService) : ICommandService<PlayerVipLevelManualCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelManualCommand command, CancellationToken cancellationToken = default)
	{
		var playerInfo = await playerRepository.GetAsync(
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		VipLevelInfo vipLevelInfo = command.VipLevel.HasValue
			? await vipLevelRepository.GetAsync(
				brandId: playerInfo.BrandId,
				level: command.VipLevel.Value,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? throw new NullReferenceException()
			: await vipLevelRepository.GetAsync(
				brandId: playerInfo.BrandId,
				id: command.VipLevelId!.Value,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? throw new NullReferenceException();

		var beforePlayerVipLevelInfo = await playerVipLevelRepository.GetAsync(
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? await fixesService.GetAsync(
				query: new PlayerVipLevelFixesQuery(
					playerInfo.PlayerCode),
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await playerRepository.UpdateAsync(
			playerCode: playerInfo.PlayerCode,
			vipLevelId: vipLevelInfo.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await playerVipLevelRepository.MergeAsync(
			playerCode: playerInfo.PlayerCode,
			vipLevelId: vipLevelInfo.Id,
			vipLevel: vipLevelInfo.Level,
			autoState: command.AutoState,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await publishService.ExecuteAsync(
			command: new PlayerVipLevelManualLogPublishCommand(
				BrandId: playerInfo.BrandId,
				PlayerCode: playerInfo.PlayerCode,
				BeforeLevel: beforePlayerVipLevelInfo.VipLevel,
				BeforeAutoState: beforePlayerVipLevelInfo.AutoState,
				AfterLevel: vipLevelInfo.Level,
				AfterAutoState: command.AutoState,
				CreatedUserId: command.CreatedUserId,
				CreatedUsername: command.CreatedUsername),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
