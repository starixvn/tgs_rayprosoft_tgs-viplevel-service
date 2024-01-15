namespace Tgs.VipLevelService.Common.MessageModels;
public record PlayerVipLevelLogRegisterMessageModel(
	Guid BrandId,
	string PlayerCode,
	int AfterLevel,
	PlayerVipLevelAutoState AfterAutoState);
