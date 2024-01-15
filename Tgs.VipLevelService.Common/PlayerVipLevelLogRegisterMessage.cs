namespace Tgs.VipLevelService.Common;
public record PlayerVipLevelLogRegisterMessage(
	Guid BrandId,
	string PlayerCode,
	int AfterLevel,
	PlayerVipLevelAutoState AfterAutoState);
