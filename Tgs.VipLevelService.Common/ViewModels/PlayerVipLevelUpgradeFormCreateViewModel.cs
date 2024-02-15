using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelUpgradeFormCreateViewModel
{
	[Required]
	public string PlayerCode { get; init; } = default!;
	[Required]
	public int VipLevel { get; init; }
}
