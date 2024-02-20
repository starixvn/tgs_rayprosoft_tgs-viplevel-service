namespace Tgs.VipLevelService.Models;
public record PlayerVipLevelExpFormSnapshotExpInfo(
	string PlayerCode,
	Guid AccumDepositId,
	decimal AccumDeposit,
	Guid AccumTurnoverId,
	decimal AccumTurnover);
