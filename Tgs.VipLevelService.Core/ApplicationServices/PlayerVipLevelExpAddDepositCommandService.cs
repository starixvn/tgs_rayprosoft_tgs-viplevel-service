using System.Net;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Polly;
using Tgs.Calculate.Clients;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
internal class PlayerVipLevelExpAddDepositCommandService(
	ILogger<PlayerVipLevelExpAddDepositCommandService> logger,
	ICryptographyUtensil utensil,
	ICalculateClient calculateClient,
	IPlayerVipLevelExpRepository playerVipLevelExpRepository,
	IPlayerVipLevelExpDepositRecordRepository playerVipLevelExpDepositRecordRepository,
	IGetService<PlayerVipLevelExpFixesQuery, PlayerVipLevelExpInfo> playerVipLevelExpFixesService) : ICommandService<PlayerVipLevelExpAddDepositCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelExpAddDepositCommand command, CancellationToken cancellationToken = default)
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

		var data = string.Concat("Form", "=", command.FormId);
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
			retryCount: 1,
			retryAttempt => TimeSpan.FromSeconds(5),
			onRetry: async (ex, retryTime, retryCount, context) =>
			{
				logger.LogWarning(ex, "LogOn:{logOn} RetryCount:{retryCount} Delay:{retryTime}",
					DateTime.Now.ToString("o"),
					retryCount,
					retryTime);
				_ = await playerVipLevelExpFixesService.GetAsync(
					query: new PlayerVipLevelExpFixesQuery(
					PlayerCode: command.PlayerCode),
					cancellationToken: cancellationToken)
				.ConfigureAwait(false);
			});

		var exchange = await retryPolicy.ExecuteAsync(
			async () => await calculateClient.ExchangeAsync(
				calculateId: playerVipLevelExpInfo.AccumDepositId,
				change: command.Amount,
				cancellationToken: cancellationToken)
			.ConfigureAwait(false))
			.ConfigureAwait(false);

		await playerVipLevelExpRepository.UpdateDepositAsync(
			playerCode: command.PlayerCode,
			accumDeposit: exchange.PostBalance,
			cancellationToken: cancellationToken)
		   .ConfigureAwait(false);

		await playerVipLevelExpDepositRecordRepository.AddAsync(
			 hash: hash,
			 playerCode: command.PlayerCode,
			 type: PlayerVipLevelExpDepositRecordType.Normal,
			 sign: command.Amount >= 0 ? 1 : -1,
			 amount: Math.Abs(command.Amount),
			 balance: exchange.PostBalance,
			 additionalData: new JsonObject
			 {
				 ["Form"] = command.FormId,
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

