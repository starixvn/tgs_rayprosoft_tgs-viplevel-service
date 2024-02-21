namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelExpAddTurnoverManualCommand(
	string PlayerCode,
	Guid FormId,
	int Sign,
	decimal Amount,
	string Remark,
	Guid OperatorUserId,
	string OperatorUsername);
