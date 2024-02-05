namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelUpgradePromotionCommand(
	string PlayerCode,
	Guid VipLevelId,
	Guid SnapshotId,
	decimal Amount);
