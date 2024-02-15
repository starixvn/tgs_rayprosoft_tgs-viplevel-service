using Tgs.SerialNumberGenerator;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class PlayerVipLevelUpgradeFormCreateGetService(
	ISerialNumberGenerator serialNumberGenerator,
	IPlayerVipLevelUpgradeFormRepository repository
	) : IGetService<PlayerVipLevelUpgradeFormCreateCommand, Guid>
{
	public async ValueTask<Guid> GetAsync(PlayerVipLevelUpgradeFormCreateCommand query, CancellationToken cancellationToken = default)
	{
		var id = await serialNumberGenerator.GenerateAsync(cancellationToken).ConfigureAwait(false);
		await repository.CreateAsync(
			id: id,
			brandId: query.BrandId,
			playerCode: query.PlayerCode,
			vipLevel: query.VipLevel,
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

		return id;
	}
}
