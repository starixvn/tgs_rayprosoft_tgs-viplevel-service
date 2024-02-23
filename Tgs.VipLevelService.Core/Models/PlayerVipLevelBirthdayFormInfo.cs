using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.Models;
public record PlayerVipLevelBirthdayFormInfo(
	Guid Id,
	Guid BrandId,
	string PlayerCode,
	int VipLevel,
	DateOnly Birthday,
	bool AutoState,
	PlayerVipLevelBirthdayFormState State,
	decimal Amount,
	decimal ActualAmount,
	string Remark,
	DateTimeOffset CreatedOn,
	Guid CreatedUserId,
	string CreatedUsername,
	DateTimeOffset UpdatedOn,
	Guid UpdatedUserId,
	string UpdatedUsername);
