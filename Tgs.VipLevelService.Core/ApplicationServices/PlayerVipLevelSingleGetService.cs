using System.Text.Json;
using Microsoft.Extensions.Logging;
using Tgs.GameWagers.Client;
using Tgs.Payment.Clients;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelSingleGetService(
	ILogger<PlayerVipLevelSingleGetService> logger,
	IPlayerRepository playerRepository,
	IPlayerVipLevelRepository playerVipLevelRepository,
	IVipLevelRepository vipLevelRepository,
	IPaymentServiceClient paymentServiceClient,
	IGameWagersClient gameWagersClient,
	IGetService<PlayerVipLevelFixesQuery, PlayerVipLevelInfo> playerVipLevelFixesService) : IGetService<PlayerVipLevelSingleQuery, PlayerVipLevelSingleResult>
{
	public async ValueTask<PlayerVipLevelSingleResult> GetAsync(PlayerVipLevelSingleQuery query, CancellationToken cancellationToken = default)
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

		var payment = await paymentServiceClient.GetPlayerPaymentOverviewAsync(
			playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
		logger.LogInformation("LogOn:{logOn} Message:This {playerCode} payment overview informationis.({paymentOverview})",
			DateTime.Now.ToString("u"),
			playerInfo.PlayerCode,
			JsonSerializer.Serialize(payment));

		var playerAccumInfo = await gameWagersClient.GameWagerStatGetPlayerAccumAsync(
			playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

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
		var nextLevelUpAccumulateBetAmount = currentLevel.LevelUpAccumulateBetAmount;
		var nextLevelUpAccumulateDeposit = currentLevel.LevelUpAccumulateDeposit;
		var fullLevel = true;
		if (nextLevel is not null)
		{
			nextVipLevelId = nextLevel.Id;
			nextVipLevel = nextLevel.Level;
			nextLevelUpAccumulateBetAmount = nextLevel.LevelUpAccumulateBetAmount;
			nextLevelUpAccumulateDeposit = nextLevel.LevelUpAccumulateDeposit;
			fullLevel = false;
		}

		return new PlayerVipLevelSingleResult(
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
			AccumDeposit: payment.TotalDepositAmount,
			AccumTurnover: playerAccumInfo.Turnover,
			LevelUpAccumulateBetAmount: nextLevelUpAccumulateBetAmount,
			LevelUpAccumulateDeposit: nextLevelUpAccumulateDeposit,
			FullLevel: fullLevel,
			NextVipLevelId: nextVipLevelId,
			NextVipLevel: nextVipLevel);
	}
}
