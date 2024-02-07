using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService.Entities;

namespace Tgs.VipLevelService.Repositories;

internal class PlayerVipLevelUpgradePromotionRepository(TgsContext context) : IPlayerVipLevelUpgradePromotionRepository
{
	public async ValueTask AddAsync(Guid id, Guid snapshotId, Guid vipLevelId, string playerCode, decimal amount, CancellationToken cancellationToken = default)
	{
		var promotion = new PlayerVipLevelUpgradePromotion
		{
			Id = id,
			PlayerVipLevelSnapshotId = snapshotId,
			VipId = vipLevelId,
			PlayerCode = playerCode,
			Amount = amount,
			CreatedOn = DateTime.UtcNow
		};
		_ = await context.PlayerVipLevelUpgradePromotions
			.AddAsync(
			entity: promotion,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false);

		_ = await context
			.SaveChangesAsync(
			cancellationToken: cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask<bool> ExistAsync(Guid vipLevelId, string playerCode, CancellationToken cancellationToken = default)
	=> await context.PlayerVipLevelUpgradePromotions
		.Where(predicate: vlp => vlp.VipId == vipLevelId && vlp.PlayerCode == playerCode)
		.Select(selector: vlp => vlp.Id)
		.AnyAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
