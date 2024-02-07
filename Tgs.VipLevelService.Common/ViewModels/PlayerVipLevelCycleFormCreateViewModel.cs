using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelCycleFormCreateViewModel
{
	[Required]
	public string PlayerCode { get; init; } = default!;
	[Required]
	public PlayerVipLevelCycleFormCycleType CycleType { get; init; }
	[Required]
	public DateOnly CycleOn { get; init; }
	[Required]
	public DateOnly FromOn { get; init; }
	[Required]
	public DateOnly ToOn { get; init; }
}
