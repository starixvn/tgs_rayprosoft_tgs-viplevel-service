using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelUpgradePromotionViewModel
{
	[Required]
	public Guid VipLevelId { get; init; }
	[Required]
	public Guid SnapshotId { get; init; }
	[Required]
	public decimal Amount { get; init; }
}
