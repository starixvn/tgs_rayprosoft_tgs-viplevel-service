namespace Tgs.VipLevelService.ApplicationServices;
public record VipLevelModifyCommand(Guid Id, VipLevelModifyModel ModifyModel, string Modifier);
