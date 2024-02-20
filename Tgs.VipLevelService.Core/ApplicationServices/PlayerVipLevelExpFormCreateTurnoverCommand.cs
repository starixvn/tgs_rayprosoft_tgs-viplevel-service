namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelExpFormCreateTurnoverCommand(
	Guid BrandId,
	string PlayerCode,
	int VipLevel,
	int Sign,
	decimal Amount,
	string Remark,
	Guid OperatorUserId,
	string OperatorUsername,
	PlayerVipLevelSingleResult PlayerVipLevel);
