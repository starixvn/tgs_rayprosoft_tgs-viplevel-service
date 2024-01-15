namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelInfoViewModel(
   string PlayerCode,
   Guid VipLevelId,
   int VipLevel,
   PlayerVipLevelAutoState AutoState);
