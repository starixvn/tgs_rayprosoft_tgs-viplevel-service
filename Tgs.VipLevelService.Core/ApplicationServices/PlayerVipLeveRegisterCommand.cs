using Tgs.VipLevelService.Common;

namespace Tgs.VipLevelService.ApplicationServices;

public record PlayerVipLeveRegisterCommand(
	string PlayerCode,
	Guid? VipLevelId,
	int? VipLevel,
	PlayerVipLevelAutoState AutoState);
