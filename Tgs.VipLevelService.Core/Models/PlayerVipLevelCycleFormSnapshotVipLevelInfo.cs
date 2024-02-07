namespace Tgs.VipLevelService.Models;
public record PlayerVipLevelCycleFormSnapshotVipLevelInfo(
	Guid VipLevelId,
	int VipLevel,
	Guid BrandId,
	decimal QuarterlyBenefit);
