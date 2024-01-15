using Tgs.VipLevelService.Common;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelSystemLogPublishCommand(
	Guid BrandId,
	string PlayerCode,
	int? BeforeLevel,
	PlayerVipLevelAutoState? BeforeAutoState,
	int AfterLevel,
	PlayerVipLevelAutoState AfterAutoState);
