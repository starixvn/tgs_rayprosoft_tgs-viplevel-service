using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.Calculate.Clients;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelExpAddTurnoverManualCommandService(
	ILogger<PlayerVipLevelExpAddTurnoverManualCommandService> logger,
	IOptions<VipLevelPolicySettingsOptions> options,
	ICryptographyUtensil utensil,
	ICalculateClient calculateClient,
	IPlayerVipLevelExpRepository playerVipLevelExpRepository,
	IPlayerVipLevelExpTurnoverRecordRepository playerVipLevelExpTurnoverRecordRepository,
	IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo> playerVipLevelExpFixesService,
	ICommandService<PlayerVipLevelCheckCommand> playerVipLevelCheckService) : ICommandService<PlayerVipLevelExpAddTurnoverManualCommand>
{
	private readonly VipLevelPolicySettingsOptions _vipLevelPolicySettings = options.Value;
	public async ValueTask ExecuteAsync(PlayerVipLevelExpAddTurnoverManualCommand command, CancellationToken cancellationToken = default)
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
			  "Type", "=", PlayerVipLevelExpTurnoverRecordType.Manual, "&",
			  "PlayerCode", "=", playerVipLevelExpInfo.PlayerCode, "&",
			  "Form", "=", command.FormId);
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
				change: command.Amount * command.Sign,
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
			 playerCode: command.PlayerCode,
			 type: PlayerVipLevelExpTurnoverRecordType.Manual,
			 sign: command.Sign,
			 amount: command.Amount,
			 balance: exchange.PostBalance,
			 additionalData: new JsonObject
			 {
				 ["PlayerCode"] = playerVipLevelExpInfo.PlayerCode,
				 ["Form"] = command.FormId,
				 ["Data"] = data
			 },
			 remark: "System addition",
			 createdOn: DateTimeOffset.Now,
			 createdUserId: command.OperatorUserId,
			 createdUsername: command.OperatorUsername,
			 cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await playerVipLevelCheckService.ExecuteAsync(
			command: new PlayerVipLevelCheckCommand(
				PlayerCode: playerVipLevelExpInfo.PlayerCode),
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
