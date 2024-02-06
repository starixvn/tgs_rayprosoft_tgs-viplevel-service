namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelBirthdayFormViewModel(
 Guid Id,
 Guid BrandId,
 string PlayerCode,
 int VipLevel,
 DateOnly Birthday,
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
