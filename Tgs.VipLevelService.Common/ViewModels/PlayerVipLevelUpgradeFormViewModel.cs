namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelUpgradeFormViewModel(
	Guid Id,
	Guid BrandId,
	string PlayerCode,
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
