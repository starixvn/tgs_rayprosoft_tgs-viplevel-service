using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelPlayerCodeGetService(
	IPlayerRepository playerRepository,
	IPlayerVipLevelRepository playerVipLevelRepository,
	IPlayerVipLevelExpRepository playerVipLevelExpRepository,
	IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo> playerVipLevelFixesService,
	ICommandService<PlayerVipLevelExpFixesCommand> playerVipLevelExpFixesService) : IGetService<PlayerVipLevelPlayerCodeQuery, PlayerVipLevelPlayerCodeResult>
{
	public async ValueTask<PlayerVipLevelPlayerCodeResult> GetAsync(PlayerVipLevelPlayerCodeQuery query, CancellationToken cancellationToken = default)
	{
		var playerInfo = await playerRepository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var playerVipLevelInfo = await playerVipLevelRepository.GetAsync(
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? await playerVipLevelFixesService.GetAsync(
				query: new PlayerVipLevelFixesQuery(
					PlayerCode: query.PlayerCode),
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var playerVipLevelExpInfo = await playerVipLevelExpRepository.GetAsync(
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (playerVipLevelExpInfo is null)
		{
			await playerVipLevelExpFixesService.ExecuteAsync(
			   command: new PlayerVipLevelExpFixesCommand(
				   PlayerCode: query.PlayerCode),
			   cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
		}

		return new PlayerVipLevelPlayerCodeResult(
			playerInfo.ResellerId,
			playerInfo.ResellerCode,
			playerInfo.LicenseeId,
			playerInfo.LicenseeCode,
			playerInfo.BrandId,
			playerInfo.BrandCode,
			playerInfo.PlayerCode,
			playerInfo.Username,
			playerVipLevelInfo.VipLevelId,
			playerVipLevelInfo.VipLevel,
			playerVipLevelInfo.AutoState,
			playerVipLevelExpInfo?.AccumDepositId ?? Guid.Empty,
			playerVipLevelExpInfo?.AccumDeposit ?? 0.0m,
			playerVipLevelExpInfo?.AccumTurnoverId ?? Guid.Empty,
			playerVipLevelExpInfo?.AccumTurnover ?? 0.0m);
	}
}
