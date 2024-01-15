namespace Tgs.VipLevelService.Common;
public record PlayerVipLevelLogManualMessage(
	Guid BrandId,
	string PlayerCode,
	int? BeforeLevel,
	PlayerVipLevelAutoState? BeforeAutoState,
	int AfterLevel,
	PlayerVipLevelAutoState AfterAutoState,
	Guid CreatedUserId,
	string CreatedUsername,
	string Remark);
