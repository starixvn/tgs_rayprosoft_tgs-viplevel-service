using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelRegisterCommandService(
	IVipLevelRepository vipLevelRepository,
	IPlayerRepository playerRepository,
	IPlayerVipLevelRepository playerVipLevelRepository,
	IPlayerVipLevelExpRepository playerVipLevelExpRepository,
	ICommandService<PlayerVipLevelRegisterLogPublishCommand> publishService) : ICommandService<PlayerVipLevelRegisterCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelRegisterCommand command, CancellationToken cancellationToken = default)
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

		await playerVipLevelRepository.MergeAsync(
			playerCode: playerInfo.PlayerCode,
			vipLevelId: vipLevelInfo.Id,
			vipLevel: vipLevelInfo.Level,
			autoState: command.AutoState,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await playerVipLevelExpRepository.CreateAsync(
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await publishService.ExecuteAsync(
			command: new PlayerVipLevelRegisterLogPublishCommand(
				BrandId: playerInfo.BrandId,
				PlayerCode: playerInfo.PlayerCode,
				AfterLevel: vipLevelInfo.Level,
				AfterAutoState: command.AutoState),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
