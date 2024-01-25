namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelExpInfoViewModel(
	string PlayerCode,
	Guid AccumDepositId,
	decimal AccumDeposit,
	Guid AccumTurnoverId,
	decimal AccumTurnover);
