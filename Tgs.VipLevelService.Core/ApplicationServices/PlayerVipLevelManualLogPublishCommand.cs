using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelManualLogPublishCommand(
	Guid BrandId,
	string PlayerCode,
	int? BeforeLevel,
	PlayerVipLevelAutoState? BeforeAutoState,
	int AfterLevel,
	PlayerVipLevelAutoState AfterAutoState,
	Guid CreatedUserId,
	string CreatedUsername,
	string Remark);
