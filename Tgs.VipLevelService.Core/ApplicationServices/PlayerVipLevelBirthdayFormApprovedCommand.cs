namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelBirthdayFormApprovedCommand(
	Guid Id,
	decimal Amount,
	string Remark,
	Guid OperatorUserId,
	string OperatorUsername);
