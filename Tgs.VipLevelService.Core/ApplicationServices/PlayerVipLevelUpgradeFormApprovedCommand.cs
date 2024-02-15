namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelUpgradeFormApprovedCommand(
	Guid Id,
	decimal Amount,
	string Remark,
	Guid OperatorUserId,
	string OperatorUsername);
