using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;
using Valhalla.MessageQueue;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelUpgradeFormCreateGetService(
	ISerialNumberGenerator serialNumberGenerator,
	IPlayerVipLevelUpgradeFormRepository repository,
	IMessageSender messageSender) : IGetService<PlayerVipLevelUpgradeFormCreateCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(PlayerVipLevelUpgradeFormCreateCommand query, CancellationToken cancellationToken = default)
	{
		var id = await serialNumberGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);

		/*
		 * 自動/手動
		 */
		var autoState = true;

		await repository.CreateAsync(
			id: id,
			brandId: query.BrandId,
			playerCode: query.PlayerCode,
			vipLevel: query.VipLevel,
			autoState: autoState,
			amount: query.Amount,
			snapshotVipLevel: new PlayerVipLevelUpgradeFormSnapshotVipLevelInfo(
				VipLevelId: query.VipLevelInfo.Id,
				VipLevel: query.VipLevelInfo.Level,
				BrandId: query.VipLevelInfo.BrandId,
				PromotionBenefit: query.VipLevelInfo.PromotionBenefit),
			createdUserId: query.OperatorUserId,
			createdUsername: query.OperatorUsername,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		/*
		 * 發送通知
		 */
		await messageSender.PublishAsync(
			subject: VipLevelServiceNatsConsts.PlayerVipLevelUpgradeFormAuto,
			data: new VipLevelFormRequest
			{
				FormId = id,
			},
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		return id;
	}
}
