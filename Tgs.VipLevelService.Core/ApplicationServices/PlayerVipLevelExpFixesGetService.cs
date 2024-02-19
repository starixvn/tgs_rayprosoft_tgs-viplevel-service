using Microsoft.Extensions.Logging;
using Tgs.Calculate.Clients;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelExpFixesGetService(
	ILogger<PlayerVipLevelExpFixesGetService> logger,
	IPlayerVipLevelExpRepository repository,
	ICalculateClient calculateClient) : IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo>
{
	public async ValueTask<PlayerVipLevelExpInfo> GetAsync(PlayerVipLevelExpFixesQuery query, CancellationToken cancellationToken = default)
	{
		await repository.CreateAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var playerVipLevelExp = await repository.GetAsync(
			playerCode: query.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException($"This player's VIP level experience does not exist.({query.PlayerCode})");

		try
		{
			await calculateClient.CreateCalculateAsync(playerVipLevelExp.AccumTurnoverId, cancellationToken).ConfigureAwait(false);
			await calculateClient.ExchangeAsync(playerVipLevelExp.AccumTurnoverId, playerVipLevelExp.AccumTurnover, cancellationToken).ConfigureAwait(false);
		}
		catch (HttpRequestException ex)
		{
			logger.LogError(ex, "LogOn:{logOn}  Mesaage:This player creates a turnover accumulation exception.({playerCode})", DateTime.Now.ToString("o"), playerVipLevelExp.PlayerCode);
		}

		try
		{
			await calculateClient.CreateCalculateAsync(playerVipLevelExp.AccumDepositId, cancellationToken).ConfigureAwait(false);
			await calculateClient.ExchangeAsync(playerVipLevelExp.AccumDepositId, playerVipLevelExp.AccumTurnover, cancellationToken).ConfigureAwait(false);
		}
		catch (HttpRequestException ex)
		{
			logger.LogWarning(ex, "LogOn:{logOn}  Mesaage:This player created a deposit accumulation exception.({playerCode})", DateTime.Now.ToString("o"), playerVipLevelExp.PlayerCode);
		}

		return playerVipLevelExp;
	}
}
