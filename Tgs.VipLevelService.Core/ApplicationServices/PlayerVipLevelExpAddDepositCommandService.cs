using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
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

		decimal balance;
		try
		{
			var exchange = await calculateClient.ExchangeAsync(
				calculateId: playerVipLevelExpInfo.AccumDepositId,
				change: command.Amount,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			await playerVipLevelExpRepository.UpdateDepositAsync(
				playerCode: command.PlayerCode,
				accumDeposit: exchange.PostBalance,
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

		await playerVipLevelExpDepositRecordRepository.AddAsync(
			 hash: hash,
			 playerCode: command.PlayerCode,
			 type: PlayerVipLevelExpDepositRecordType.Normal,
			 sign: command.Amount >= 0 ? 1 : -1,
			 amount: Math.Abs(command.Amount),
			 balance: balance,
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

