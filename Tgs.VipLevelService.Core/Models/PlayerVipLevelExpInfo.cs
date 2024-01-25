namespace Tgs.VipLevelService.Models;
public record PlayerVipLevelExpInfo(
	string PlayerCode,
	Guid AccumDepositId,
	decimal AccumDeposit,
	Guid AccumTurnoverId,
	decimal AccumTurnover);
