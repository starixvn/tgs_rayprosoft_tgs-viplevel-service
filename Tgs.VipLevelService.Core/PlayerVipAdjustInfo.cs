namespace Tgs.VipLevelService;
public record PlayerVipAdjustInfo(
	DateTime CreatedOn,
	Guid OldVipId,
	Guid NewVipId);
