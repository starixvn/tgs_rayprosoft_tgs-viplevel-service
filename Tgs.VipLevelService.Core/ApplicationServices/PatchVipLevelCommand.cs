namespace Tgs.VipLevelService.ApplicationServices;
public record PatchVipLevelCommand(Guid Id, VipLevelUpdate ModifyModel, string Modifier);
