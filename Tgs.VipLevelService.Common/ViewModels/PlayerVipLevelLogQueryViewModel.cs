namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelLogQueryViewModel(
	PlayerVipLevelLogType Type,
	int BeforeLevel,
	Guid BeforeLevelId,
	PlayerVipLevelAutoState BeforeAutoState,
	int AfterLevel,
	Guid AfterLevelId,
	PlayerVipLevelAutoState AfterAutoState,
	DateTimeOffset CreatedOn,
	Guid CreatedUserId,
	string CreatedUsername,
	string Remark);
