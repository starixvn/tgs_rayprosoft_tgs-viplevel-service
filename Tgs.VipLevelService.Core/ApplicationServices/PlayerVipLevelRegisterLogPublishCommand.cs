using Tgs.VipLevelService.Common;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelRegisterLogPublishCommand(
	Guid BrandId,
	string PlayerCode,
	int AfterLevel,
	PlayerVipLevelAutoState AfterAutoState);
