namespace Tgs.VipLevelService.Common.MessageModels;
public record PlayerVipLevelLogSystemMessageModel(
	Guid BrandId,
	string PlayerCode,
	int? BeforeLevel,
	PlayerVipLevelAutoState? BeforeAutoState,
	int AfterLevel,
	PlayerVipLevelAutoState AfterAutoState);
