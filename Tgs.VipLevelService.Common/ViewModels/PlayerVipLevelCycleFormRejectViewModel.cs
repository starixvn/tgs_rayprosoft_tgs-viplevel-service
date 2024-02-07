using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelCycleFormRejectViewModel
{
	public string Remark { get; init; } = default!;
}
