namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelExpFormApprovedCommand(
	Guid Id,
	decimal Amount,
	string Remark,
	Guid OperatorUserId,
	string OperatorUsername);
