using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Calculate.Clients;
using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelExpCreateCommandService(
	ILogger<PlayerVipLevelExpFixesGetService> logger,
	IPlayerVipLevelExpRepository repository,
	ICalculateClient calculateClient) : ICommandService<PlayerVipLevelExpCreateCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelExpCreateCommand command, CancellationToken cancellationToken = default)
	{
		await repository.CreateAsync(
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var playerVipLevelExp = await repository.GetAsync(
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException($"This player's VIP level experience does not exist.({command.PlayerCode})");

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
	}
}
