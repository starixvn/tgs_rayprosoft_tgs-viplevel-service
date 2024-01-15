namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelLogInfoViewModel(
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