using Tgs.VipLevelService.Common;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLeveRegisterLogPublishCommand(
	Guid BrandId,
	string PlayerCode,
	int AfterLevel,
	PlayerVipLevelAutoState AfterAutoState);
