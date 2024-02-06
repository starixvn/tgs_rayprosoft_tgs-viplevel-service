namespace Tgs.VipLevelService.Models;
public record PlayerVipLevelBirthdaySnapshotVipLevelInfo(
	Guid VipLevelId,
	int VipLevel,
	Guid BrandId,
	decimal BirthBenefit);
