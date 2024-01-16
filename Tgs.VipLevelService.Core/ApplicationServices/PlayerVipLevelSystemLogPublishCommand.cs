using Tgs.VipLevelService.Common;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelSystemLogPublishCommand(
	Guid BrandId,
	string PlayerCode,
	int? BeforeLevel,
	int AfterLevel,
	PlayerVipLevelAutoState AutoState);
