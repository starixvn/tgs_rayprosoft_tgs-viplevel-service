using Tgs.ServiceUtility;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelFixesGetService(
	IVipLevelRepository vipLevelRepository,
	IPlayerRepository playerRepository,
	IPlayerVipLevelRepository playerVipLevelRepository) : IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo>
{
	public async ValueTask<PlayerVipLevelInfo> GetAsync(PlayerVipLevelFixesQuery query, CancellationToken cancellationToken = default)
	{
		var playerVipLevelInfo = await playerVipLevelRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (playerVipLevelInfo is null)
		{
			var playerVipIdInfo = await playerRepository.GetVipIdAsync(
				playerCode: query.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			var vipLevelInfo = await vipLevelRepository.GetAsync(
				brandId: playerVipIdInfo.BrandId,
				id: playerVipIdInfo.VipId,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? throw new NullReferenceException();

			await playerVipLevelRepository.CreateAsync(
				playerCode: playerVipIdInfo.PlayerCode,
				vipLevelId: vipLevelInfo.Id,
				vipLevel: vipLevelInfo.Level,
				autoState: PlayerVipLevelAutoState.Enable,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			playerVipLevelInfo = await playerVipLevelRepository.GetAsync(
				playerCode: query.PlayerCode,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false)
				?? throw new NullReferenceException($"This player's VipLevel does not exist.({query.PlayerCode})");
		}

		return playerVipLevelInfo;
	}
}
