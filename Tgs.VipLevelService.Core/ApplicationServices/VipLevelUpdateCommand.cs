using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
public record VipLevelUpdateCommand(Guid Id, VipLevelUpdate Update, string Modifier);
