namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelExpAddDepositManualCommand(
	string PlayerCode,
	string FormId,
	int Sign,
	decimal Amount);
