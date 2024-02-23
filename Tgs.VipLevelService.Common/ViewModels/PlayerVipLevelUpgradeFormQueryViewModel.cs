namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelUpgradeFormQueryViewModel(
	Guid Id,
	Guid BrandId,
	string PlayerCode,
	string Username,
	int VipLevel,
	bool AutoState,
	PlayerVipLevelUpgradeFormState State,
	decimal Amount,
	decimal ActualAmount,
	string Remark,
	DateTimeOffset CreatedOn,
	Guid CreatedUserId,
	string CreatedUsername,
	DateTimeOffset UpdatedOn,
	Guid UpdatedUserId,
	string UpdatedUsername);
