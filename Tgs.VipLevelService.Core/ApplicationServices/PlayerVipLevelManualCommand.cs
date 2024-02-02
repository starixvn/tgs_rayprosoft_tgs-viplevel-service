using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.ApplicationServices;

public record PlayerVipLevelManualCommand(
	string PlayerCode,
	Guid? VipLevelId,
	int? VipLevel,
	PlayerVipLevelAutoState AutoState,
	Guid CreatedUserId,
	string CreatedUsername,
	string Remark);
