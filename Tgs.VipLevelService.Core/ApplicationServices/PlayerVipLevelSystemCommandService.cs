using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelSystemCommandService(
	IVipLevelRepository vipLevelRepository,
	IPlayerRepository playerRepository,
	IPlayerVipLevelRepository playerVipLevelRepository,
	IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo> fixesService,
	ICommandService<PlayerVipLevelSystemLogPublishCommand> publishService) : ICommandService<PlayerVipLevelSystemCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelSystemCommand command, CancellationToken cancellationToken = default)
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

		await playerVipLevelRepository.UpdateSystemAsync(
			playerCode: playerInfo.PlayerCode,
			vipLevelId: vipLevelInfo.Id,
			vipLevel: vipLevelInfo.Level,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await publishService.ExecuteAsync(
			command: new PlayerVipLevelSystemLogPublishCommand(
				BrandId: playerInfo.BrandId,
				PlayerCode: playerInfo.PlayerCode,
				BeforeLevel: beforePlayerVipLevelInfo.VipLevel,
				AfterLevel: vipLevelInfo.Level,
				AutoState: beforePlayerVipLevelInfo.AutoState),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
