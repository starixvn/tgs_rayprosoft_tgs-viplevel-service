using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelBirthdayFormRejectViewModel
{
	public string Remark { get; init; } = default!;
}
