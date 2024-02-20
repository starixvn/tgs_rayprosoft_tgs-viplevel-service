using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.Models;
public record PlayerVipLevelExpFormInfo(
	Guid Id,
	Guid BrandId,
	string PlayerCode,
	int VipLevel,
	PlayerVipLevelExpFormType Type,
	bool AutoState,
	PlayerVipLevelExpFormState State,
	int Sign,
	decimal Amount,
	decimal ActualAmount,
	string Remark,
	DateTimeOffset CreatedOn,
	Guid CreatedUserId,
	string CreatedUsername,
	DateTimeOffset UpdatedOn,
	Guid UpdatedUserId,
	string UpdatedUsername);
