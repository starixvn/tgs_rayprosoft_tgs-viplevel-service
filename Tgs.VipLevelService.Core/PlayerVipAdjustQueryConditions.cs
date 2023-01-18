namespace Tgs.VipLevelService;
public record PlayerVipAdjustQueryConditions(
	Guid BrandId,
	string PlayerCode,
	(DateTimeOffset Begin, DateTimeOffset End)? CreateOnRange);
