namespace Tgs.VipLevelService.ApplicationServices;
public record CreateVipLevelCommand(Guid BrandId, VipLevelCreateModel CreateModel, string Modifier);
