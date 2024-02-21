namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelExpAddDepositManualCommand(
	string PlayerCode,
	Guid FormId,
	int Sign,
	decimal Amount,
	string Remark,
	Guid OperatorUserId,
	string OperatorUsername);
