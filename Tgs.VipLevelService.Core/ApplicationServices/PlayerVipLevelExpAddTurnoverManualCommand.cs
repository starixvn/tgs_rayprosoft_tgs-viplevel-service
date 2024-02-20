namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelExpAddTurnoverManualCommand(
	string PlayerCode,
	Guid FormId,
	int Sign,
	decimal Amount,
	Guid OperatorUserId,
	string OperatorUsername);
