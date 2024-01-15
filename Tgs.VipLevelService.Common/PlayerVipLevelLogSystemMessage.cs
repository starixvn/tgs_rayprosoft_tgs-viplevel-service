namespace Tgs.VipLevelService.Common;
public record PlayerVipLevelLogSystemMessage(
	Guid BrandId,
	string PlayerCode,
	int? BeforeLevel,
	PlayerVipLevelAutoState? BeforeAutoState,
	int AfterLevel,
	PlayerVipLevelAutoState AfterAutoState);
