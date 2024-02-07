namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelCycleFormApprovedCommand(
	Guid Id,
	decimal Amount,
	string Remark,
	Guid OperatorUserId,
	string OperatorUsername);
