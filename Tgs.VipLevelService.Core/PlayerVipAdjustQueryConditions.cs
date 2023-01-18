namespace Tgs.VipLevelService;
public record PlayerVipAdjustQueryConditions(
	string PlayerCode,
	(DateTimeOffset Begin, DateTimeOffset End)? CreateOnRange);
