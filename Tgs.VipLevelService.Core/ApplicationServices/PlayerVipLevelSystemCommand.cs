namespace Tgs.VipLevelService.ApplicationServices;

public record PlayerVipLevelSystemCommand(
	string PlayerCode,
	Guid? VipLevelId,
	int? VipLevel);
