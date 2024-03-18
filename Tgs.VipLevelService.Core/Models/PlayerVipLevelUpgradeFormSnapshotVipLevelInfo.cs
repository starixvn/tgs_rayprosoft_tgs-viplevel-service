namespace Tgs.VipLevelService.Models;
public record PlayerVipLevelUpgradeFormSnapshotVipLevelInfo(
	Guid VipLevelId,
	int VipLevel,
	Guid BrandId,
	decimal AccumDeposit,
	decimal AccumTurnover,
	decimal LevelUpAccumulateBetAmount,
	decimal LevelUpAccumulateDeposit,
	decimal PromotionBenefit);
