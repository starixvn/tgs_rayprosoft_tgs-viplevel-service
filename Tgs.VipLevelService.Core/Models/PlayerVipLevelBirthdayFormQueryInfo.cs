using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.Models;
public record PlayerVipLevelBirthdayFormQueryInfo(
	Guid Id,
	Guid BrandId,
	string PlayerCode,
	string Username,
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
