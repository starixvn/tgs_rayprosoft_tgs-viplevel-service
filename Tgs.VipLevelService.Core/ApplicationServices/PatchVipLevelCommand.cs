namespace Tgs.VipLevelService.ApplicationServices;
public record PatchVipLevelCommand(Guid Id, VipLevelModifyModel ModifyModel, string Modifier);
