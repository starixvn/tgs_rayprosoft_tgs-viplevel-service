using Microsoft.Extensions.Logging;
using Tgs.CashService.Client;
using Tgs.SerialNumberGenerator;
using Tgs.Service.PlayerBuckets;
using Tgs.ServiceUtility;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelUpgradePromotionCommandService(
	ILogger<PlayerVipLevelUpgradePromotionCommandService> logger,
	IPlayerRepository playerRepository,
	IPlayerVipLevelUpgradePromotionRepository playerVipLevelUpgradePromotionRepository,
	ISerialNumberGenerator serialNumberGenerator,
	IPlayerBucketClient playerBucketClient,
	ICashServiceClient cashServiceClient) : ICommandService<PlayerVipLevelUpgradePromotionCommand>
{
	public async ValueTask ExecuteAsync(PlayerVipLevelUpgradePromotionCommand command, CancellationToken cancellationToken = default)
	{
		var playerInfo = await playerRepository.GetAsync(
			playerCode: command.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (await playerVipLevelUpgradePromotionRepository.ExistAsync(
			vipLevelId: command.VipLevelId,
			playerCode: playerInfo.PlayerCode,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false))
		{
			logger.LogWarning("LogOn:{logOn} Message:This player has already been sent promotions.({command})",
			DateTime.Now.ToString("u"),
			command);
			return;
		}

		var id = await serialNumberGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);

		await playerVipLevelUpgradePromotionRepository.AddAsync(
			id: id,
			snapshotId: command.SnapshotId,
			vipLevelId: command.VipLevelId,
			playerCode: command.PlayerCode,
			amount: command.Amount,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		if (command.Amount > 0)
		{
			_ = await playerBucketClient.CreateBucketAsync(
				playerCode: playerInfo.PlayerCode,
				amount: command.Amount,
				type: BucketType.Vip,
				externalId: id.ToString(),
				remark: string.Empty,
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);

			_ = await cashServiceClient.PromotionVipLevelUpgradeAsync(
				walletId: playerInfo.WalletId,
				amount: command.Amount,
				description: string.Empty,
				externalId: id.ToString(),
				cancellationToken: cancellationToken)
				.ConfigureAwait(false);
		}
	}
}
