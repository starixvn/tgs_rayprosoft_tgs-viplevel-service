namespace Tgs.VipLevelService;
public record PlayerVipAdjustInfo(
	DateTime CreatedOn,
	int OldLevel,
	int NewLevel);
