using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelExpFormApprovedViewModel
{
	[Required()]
	[Range(minimum: 0, maximum: int.MaxValue)]
	public decimal Amount { get; init; } = 0.0m;

	public string Remark { get; init; } = default!;
}
