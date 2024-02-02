using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.ApplicationServices;

public record PlayerVipLevelRegisterCommand(
	string PlayerCode,
	Guid? VipLevelId,
	int? VipLevel,
	PlayerVipLevelAutoState AutoState);
