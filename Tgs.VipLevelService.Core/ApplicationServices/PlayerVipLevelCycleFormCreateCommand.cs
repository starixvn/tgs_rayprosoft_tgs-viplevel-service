using Tgs.VipLevelService.Core.Protos;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelCycleFormCreateCommand(
	Guid BrandId,
	string PlayerCode,
	int VipLevel,
	PlayerVipLevelCycleFormCycleType CycleType,
	DateOnly CycleOn,
	DateOnly FromOn,
	DateOnly ToOn,
	decimal Amount,
	Guid OperatorUserId,
	string OperatorUsername,
	VipLevelInfo VipLevelInfo);