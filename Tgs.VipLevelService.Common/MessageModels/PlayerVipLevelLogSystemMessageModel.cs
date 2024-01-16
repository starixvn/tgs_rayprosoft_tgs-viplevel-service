namespace Tgs.VipLevelService.Common.MessageModels;
public record PlayerVipLevelLogSystemMessageModel(
	Guid BrandId,
	string PlayerCode,
	int? BeforeLevel,
	int AfterLevel,
	PlayerVipLevelAutoState AutoState);
