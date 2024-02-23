using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.Models;
public record PlayerVipLevelCycleFormQueryInfo(
	Guid Id,
	Guid BrandId,
	string PlayerCode,
	string Username,
	int VipLevel,
	PlayerVipLevelCycleFormCycleType CycleType,
	DateOnly CycleOn,
	DateOnly FromOn,
	DateOnly ToOn,
	bool AutoState,
	PlayerVipLevelCycleFormState State,
	decimal Amount,
	decimal ActualAmount,
	string Remark,
	DateTimeOffset CreatedOn,
	Guid CreatedUserId,
	string CreatedUsername,
	DateTimeOffset UpdatedOn,
	Guid UpdatedUserId,
	string UpdatedUsername);
