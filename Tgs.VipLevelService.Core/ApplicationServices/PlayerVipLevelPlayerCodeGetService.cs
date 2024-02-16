using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelPlayerCodeGetService(
	IPlayerRepository playerRepository,
	IPlayerVipLevelRepository playerVipLevelRepository,
	IPlayerVipLevelExpRepository playerVipLevelExpRepository,
	IVipLevelRepository vipLevelRepository,
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

		var currentLevel = await vipLevelRepository.GetAsync(
			playerInfo.BrandId,
			playerVipLevelInfo.VipLevel,
			cancellationToken).ConfigureAwait(false)
			?? throw new NullReferenceException($"This brand({playerInfo.BrandCode}) VIP level({playerVipLevelInfo.VipLevel}) does not exist.");

		var nextLevel = await vipLevelRepository.GetAsync(
			playerInfo.BrandId,
			playerVipLevelInfo.VipLevel + 1,
			cancellationToken).ConfigureAwait(false);

		var nextVipLevelId = playerVipLevelInfo.VipLevelId;
		var nextVipLevel = playerVipLevelInfo.VipLevel;
		var fullLevel = true;
		if (nextLevel is not null)
		{
			nextVipLevelId = nextLevel.Id;
			nextVipLevel = nextLevel.Level;
			fullLevel = false;
		}

		return new PlayerVipLevelPlayerCodeResult(
			ResellerId: playerInfo.ResellerId,
			ResellerCode: playerInfo.ResellerCode,
			LicenseeId: playerInfo.LicenseeId,
			LicenseeCode: playerInfo.LicenseeCode,
			BrandId: playerInfo.BrandId,
			BrandCode: playerInfo.BrandCode,
			PlayerCode: playerInfo.PlayerCode,
			Username: playerInfo.Username,
			VipLevelId: playerVipLevelInfo.VipLevelId,
			VipLevel: playerVipLevelInfo.VipLevel,
			AutoState: playerVipLevelInfo.AutoState,
			AccumDepositId: playerVipLevelExpInfo?.AccumDepositId ?? Guid.Empty,
			AccumDeposit: playerVipLevelExpInfo?.AccumDeposit ?? 0.0m,
			AccumTurnoverId: playerVipLevelExpInfo?.AccumTurnoverId ?? Guid.Empty,
			AccumTurnover: playerVipLevelExpInfo?.AccumTurnover ?? 0.0m,
			LevelUpAccumulateBetAmount: currentLevel.LevelUpAccumulateBetAmount,
			LevelUpAccumulateDeposit: currentLevel.LevelUpAccumulateDeposit,
			FullLevel: fullLevel,
			NextVipLevelId: nextVipLevelId,
			NextVipLevel: nextVipLevel);
	}
}
