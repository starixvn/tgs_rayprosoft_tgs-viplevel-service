using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelExpFormCreateViewModel
{
	[Required]
	public string PlayerCode { get; init; } = default!;
	[Required]
	[Range(minimum: 0, maximum: int.MaxValue)]
	public decimal Amount { get; init; } = 0.0m;
	[Required]
	public string Remark { get; init; } = default!;
}
