using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.ViewModels;

public class VipLevelSnapshotCreateViewModel
{
	[Required]
	public Guid BrandId { get; init; }

	[Required]
	public DateTimeOffset EffectiveOn { get; init; } = DateTimeOffset.Now;

	[Required]
	public Guid SnapshotUserId { get; init; }

	[Required]
	public string SnapshotUsername { get; init; } = default!;
}
