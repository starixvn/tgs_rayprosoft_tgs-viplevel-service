using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
public record CreateVipLevelCommand(Guid BrandId, VipLevelCreate CreateModel, string Modifier);
