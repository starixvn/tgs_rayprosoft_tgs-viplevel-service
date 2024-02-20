using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.Calculate.Clients;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelExpAddTurnoverCommandService(
	ILogger<PlayerVipLevelExpAddTurnoverCommandService> logger,
	IOptions<VipLevelPolicySettingsOptions> options,
	ICryptographyUtensil utensil,
	ICalculateClient calculateClient,
	IPlayerVipLevelExpRepository playerVipLevelExpRepository,
	IPlayerVipLevelExpTurnoverRecordRepository playerVipLevelExpTurnoverRecordRepository,
	IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo> playerVipLevelExpFixesService,
	ICommandService<PlayerVipLevelCheckCommand> playerVipLevelCheckService) : ICommandService<PlayerVipLevelExpAddTurnoverCommand>
{
	private readonly VipLevelPolicySettingsOptions _vipLevelPolicySettings = options.Value;
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
			"Type", "=", PlayerVipLevelExpTurnoverRecordType.Normal, "&",
			"PlayerCode", "=", playerVipLevelExpInfo.PlayerCode, "&",
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

		var retryPolicy = Policy.Handle<HttpRequestException>()
			.WaitAndRetryAsync(
			retryCount: _vipLevelPolicySettings.PlayerVipLevelExpTurnoverRetryCount,
			retryAttempt => TimeSpan.FromSeconds(_vipLevelPolicySettings.PlayerVipLevelExpTurnoverRetryAttemptSec),
			onRetry: async (ex, retryTime, retryCount, context) =>
			{
				logger.LogWarning(ex, "LogOn:{logOn} RetryCount:{retryCount} Delay:{retryTime}",
					DateTime.Now.ToString("o"),
					retryCount,
					retryTime);
				_ = await playerVipLevelExpFixesService.GetAsync(
					query: new PlayerVipLevelExpFixesQuery(
						PlayerCode: playerVipLevelExpInfo.PlayerCode),
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			});

		var exchange = await retryPolicy.ExecuteAsync(
			 () => calculateClient.ExchangeAsync(
				calculateId: playerVipLevelExpInfo.AccumTurnoverId,
				change: command.Amount,
				cancellationToken: cancellationToken)
			 .AsTask())
			.ConfigureAwait(false);

		await playerVipLevelExpRepository.UpdateTurnoverAsync(
				playerCode: playerVipLevelExpInfo.PlayerCode,
				accumTurnover: exchange.PostBalance,
				cancellationToken: cancellationToken)
			   .ConfigureAwait(false);

		await playerVipLevelExpTurnoverRecordRepository.AddAsync(
			hash: hash,
			 playerCode: playerVipLevelExpInfo.PlayerCode,
			 type: PlayerVipLevelExpTurnoverRecordType.Normal,
			 sign: command.Amount >= 0 ? 1 : -1,
			 amount: Math.Abs(command.Amount),
			 balance: exchange.PostBalance,
			 additionalData: new JsonObject
			 {
				 ["PlayerCode"] = playerVipLevelExpInfo.PlayerCode,
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

		await playerVipLevelCheckService.ExecuteAsync(
			command: new PlayerVipLevelCheckCommand(
				PlayerCode: playerVipLevelExpInfo.PlayerCode),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
