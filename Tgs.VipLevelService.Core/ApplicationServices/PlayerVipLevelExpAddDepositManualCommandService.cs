using System.Net;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Tgs.Calculate.Clients;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelExpAddDepositManualCommandService(
	ILogger<PlayerVipLevelExpAddDepositManualCommandService> logger,
	IOptions<VipLevelPolicySettingsOptions> options,
	ICryptographyUtensil utensil,
	ICalculateClient calculateClient,
	IPlayerVipLevelExpRepository playerVipLevelExpRepository,
	IPlayerVipLevelExpDepositRecordRepository playerVipLevelExpDepositRecordRepository,
	IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo> playerVipLevelExpFixesService,
	ICommandService<PlayerVipLevelCheckCommand> playerVipLevelCheckService) : ICommandService<PlayerVipLevelExpAddDepositManualCommand>
{
	private readonly VipLevelPolicySettingsOptions _vipLevelPolicySettings = options.Value;
	public async ValueTask ExecuteAsync(PlayerVipLevelExpAddDepositManualCommand command, CancellationToken cancellationToken = default)
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

		var parameters = new Dictionary<string, string>
		{
			{ "Type", PlayerVipLevelExpDepositRecordType.Manual.ToString() },
			{ "PlayerCode", playerVipLevelExpInfo.PlayerCode },
			{ "Form",command.FormId.ToString() },
		};
		var data = string.Join("&", parameters.Select(kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)));
		var hash = utensil.Sha512(data);

		if (await playerVipLevelExpDepositRecordRepository.ExistAsync(
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
			retryCount: _vipLevelPolicySettings.PlayerVipLevelExpDepositRetryCount,
			retryAttempt => TimeSpan.FromSeconds(_vipLevelPolicySettings.PlayerVipLevelExpDepositRetryAttemptSec),
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
				calculateId: playerVipLevelExpInfo.AccumDepositId,
				change: command.Amount * command.Sign,
				cancellationToken: cancellationToken)
			.AsTask())
			.ConfigureAwait(false);

		await playerVipLevelExpRepository.UpdateDepositAsync(
			playerCode: playerVipLevelExpInfo.PlayerCode,
			accumDeposit: exchange.PostBalance,
			cancellationToken: cancellationToken)
		   .ConfigureAwait(false);

		await playerVipLevelExpDepositRecordRepository.AddAsync(
			 hash: hash,
			 playerCode: playerVipLevelExpInfo.PlayerCode,
			 type: PlayerVipLevelExpDepositRecordType.Manual,
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

