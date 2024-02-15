namespace Tgs.VipLevelService.Models;
public record PlayerVipLevelUpgradeFormSnapshotVipLevelInfo(
	Guid VipLevelId,
	int VipLevel,
	Guid BrandId,
	decimal PromotionBenefit);
