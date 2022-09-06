namespace Tgs.VipLevelService.ApplicationServices;
public record CreateVipLevelCommand(Guid BrandId, VipLevelModifyModel CreateModel, string Modifier);
