using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;
public record VipLevelCreateCommand(Guid BrandId, VipLevelCreate CreateModel, string Modifier);
