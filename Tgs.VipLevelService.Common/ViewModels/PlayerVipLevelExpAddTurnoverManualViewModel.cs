using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelExpAddTurnoverManualViewModel
{
	[Required]
	public string FormId { get; init; } = default!;
	[Required]
	[Range(minimum: 0, maximum: int.MaxValue)]
	public decimal Amount { get; init; }
}
