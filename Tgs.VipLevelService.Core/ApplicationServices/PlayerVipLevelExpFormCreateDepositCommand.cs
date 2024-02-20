using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelExpFormCreateDepositCommand(
	Guid BrandId,
	string PlayerCode,
	int VipLevel,
	int Sign,
	decimal Amount,
	string Remark,
	Guid OperatorUserId,
	string OperatorUsername,
	PlayerVipLevelSingleResult PlayerVipLevel);
