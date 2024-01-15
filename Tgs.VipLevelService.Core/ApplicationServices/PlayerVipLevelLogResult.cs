using Tgs.VipLevelService.Common;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelLogResult(
	PlayerVipLevelLogType Type,
	int BeforeLevel,
	Guid BeforeLevelId,
	PlayerVipLevelAutoState BeforeAutoState,
	int AfterLevel,
	Guid AfterLevelId,
	PlayerVipLevelAutoState AfterAutoState,
	DateTimeOffset CreatedOn,
	Guid CreatedUserId,
	string CreatedUsername);
