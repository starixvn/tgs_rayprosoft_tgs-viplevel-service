using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelExpFormRejectViewModel
{
	public string Remark { get; init; } = default!;
}
