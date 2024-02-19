using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Tgs.Calculate.Clients;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelExpAddTurnoverCommandService(
	ILogger<PlayerVipLevelExpAddTurnoverCommandService> logger,
	ICryptographyUtensil utensil,
	ICalculateClient calculateClient,
	IPlayerVipLevelExpRepository playerVipLevelExpRepository,
	IPlayerVipLevelExpTurnoverRecordRepository playerVipLevelExpTurnoverRecordRepository,
	IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo> playerVipLevelExpFixesService) : ICommandService<PlayerVipLevelExpAddTurnoverCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelExpAddTurnoverCommand command, CancellationToken cancellationToken = default)
	{
		var playerVipLevelExpInfo = await playerVipLevelExpRepository.GetAsync(
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? await playerVipLevelExpFixesService.GetAsync(
				query: new PlayerVipLevelExpFixesQuery(
					PlayerCode: command.PlayerCode),
				cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		var data = string.Concat(
			"GameProviderId", "=", command.GameProviderId, "&",
			"GameId", "=", command.GameId, "&",
			"WalletId", "=", command.WalletId, "&",
			"ExternalWagerId", "=", command.ExternalWagerId);
		var hash = utensil.Sha512(data);

		if (await playerVipLevelExpTurnoverRecordRepository.ExistAsync(
			hash: hash,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
		{
			logger.LogWarning("LogOn:{logOn} Mesaage:This process has been tagged. Data:({data})",
				DateTime.Now.ToString("o"),
				data);

			return;
		}

		decimal balance;
		try
		{
			var exchange = await calculateClient.ExchangeAsync(
				calculateId: playerVipLevelExpInfo.AccumTurnoverId,
				change: command.Amount,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			await playerVipLevelExpRepository.UpdateTurnoverAsync(
				playerCode: command.PlayerCode,
				accumTurnover: exchange.PostBalance,
				cancellationToken: cancellationToken)
			   .ConfigureAwait(false);

			balance = exchange.PostBalance;
		}
		catch (HttpRequestException ex)
		{
			logger.LogError(ex, "LogOn:{logOn}  Mesaage:This player calls the calculator service abnormally.({playerCode}|{playerVipLevelExpInfo.AccumDepositId})", DateTime.Now.ToString("o"), playerVipLevelExpInfo.PlayerCode, playerVipLevelExpInfo.AccumDepositId);
			throw;
		}
		catch (Exception)
		{
			throw;
		}

		await playerVipLevelExpTurnoverRecordRepository.AddAsync(
			hash: hash,
			 playerCode: command.PlayerCode,
			 type: PlayerVipLevelExpTurnoverRecordType.Normal,
			 sign: command.Amount >= 0 ? 1 : -1,
			 amount: Math.Abs(command.Amount),
			 balance: balance,
			 additionalData: new JsonObject
			 {
				 ["GameProviderId"] = command.GameProviderId,
				 ["GameId"] = command.GameId,
				 ["WalletId"] = command.WalletId,
				 ["ExternalWagerId"] = command.ExternalWagerId,
				 ["WagerId"] = command.WagerId,
				 ["Data"] = data
			 },
			 remark: "System addition",
			 createdOn: DateTimeOffset.Now,
			 createdUserId: Guid.Empty,
			 createdUsername: string.Empty,
			 cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
