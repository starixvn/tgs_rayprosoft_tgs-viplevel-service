using System.Text.Json.Nodes;
using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.Service.PlayerBuckets;
using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelUpgradeFormApprovedCommandService(
	ILogger<PlayerVipLevelUpgradeFormApprovedCommandService> logger,
	IPlayerVipLevelUpgradeFormRepository playerVipLevelUpgradeFormRepository,
	IPlayerRepository playerRepository,
	IPlayerBucketClient playerBucketClient,
	ICashServiceClient cashServiceClient) : ICommandService<PlayerVipLevelUpgradeFormApprovedCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelUpgradeFormApprovedCommand command, CancellationToken cancellationToken = default)
	{
		var form = await playerVipLevelUpgradeFormRepository.GetAsync(
			id: command.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException($"This form cannot be found.{command.Id}");

		if (form.State is Core.Protos.PlayerVipLevelUpgradeFormState.Approved or Core.Protos.PlayerVipLevelUpgradeFormState.Reject)
		{
			logger.LogWarning("LogOn:{logOn} Message:This form({id}) has been reviewed.({state})",
				DateTime.Now.ToString("u"),
				form.Id,
				form.State);
			return;
		}

		var playerInfo = await playerRepository.GetAsync(
			playerCode: form.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		await playerVipLevelUpgradeFormRepository.ApprovedAsync(
			id: form.Id,
			amount: command.Amount,
			remark: command.Remark,
			updatedUserId: command.OperatorUserId,
			updatedUsername: command.OperatorUsername,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (command.Amount > 0)
		{
			var bucketId = await playerBucketClient.CreateBucketAsync(
				playerCode: playerInfo.PlayerCode,
				amount: command.Amount,
				type: BucketType.Vip,
				externalId: form.Id.ToString(),
				remark: command.Remark,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			_ = await cashServiceClient.PromotionVipLevelUpgradeAsync(
				walletId: playerInfo.WalletId,
				amount: command.Amount,
				description: new JsonObject
				{
					["BucketId"] = bucketId
				}.ToJsonString(),
				externalId: form.Id.ToString(),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
