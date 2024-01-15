namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelLogCountQuery(
	string PlayerCode,
	DateTimeOffset StartOn,
	DateTimeOffset EndOn);
