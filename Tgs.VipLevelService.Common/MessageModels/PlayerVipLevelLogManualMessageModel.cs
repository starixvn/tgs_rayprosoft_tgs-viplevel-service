namespace Tgs.VipLevelService.Common.MessageModels;
public record PlayerVipLevelLogManualMessageModel(
	Guid BrandId,
	string PlayerCode,
	int? BeforeLevel,
	PlayerVipLevelAutoState? BeforeAutoState,
	int AfterLevel,
	PlayerVipLevelAutoState AfterAutoState,
	Guid CreatedUserId,
	string CreatedUsername,
	string Remark);
