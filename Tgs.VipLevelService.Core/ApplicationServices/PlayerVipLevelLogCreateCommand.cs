using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelLogCreateCommand(
	Guid BrandId,
	string PlayerCode,
	PlayerVipLevelLogType Type,
	int? BeforeLevel,
	PlayerVipLevelAutoState? BeforeAutoState,
	int AfterLevel,
	PlayerVipLevelAutoState AfterAutoState,
	Guid CreatedUserId,
	string CreatedUsername,
	string Remark);
