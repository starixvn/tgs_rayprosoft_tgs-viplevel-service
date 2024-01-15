namespace Tgs.VipLevelService.ApplicationServices;
public record VipLevelCreateCommand(Guid BrandId, VipLevelCreateModel CreateModel, string Modifier);