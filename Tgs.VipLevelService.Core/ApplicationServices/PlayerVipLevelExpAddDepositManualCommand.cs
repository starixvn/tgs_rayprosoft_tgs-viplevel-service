namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelExpAddDepositManualCommand(
	string PlayerCode,
	Guid FormId,
	int Sign,
	decimal Amount,
	Guid OperatorUserId,
	string OperatorUsername);
