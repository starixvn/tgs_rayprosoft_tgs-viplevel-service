namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelExpAddTurnoverManualCommand(
	string PlayerCode,
	string FormId,
	int Sign,
	decimal Amount);
