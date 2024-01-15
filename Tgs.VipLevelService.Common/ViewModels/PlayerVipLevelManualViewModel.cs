using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelManualViewModel
{
	public Guid? VipLevelId { get; init; }
	public int? VipLevel { get; init; }
	[Required]
	public PlayerVipLevelAutoState AutoState { get; init; }
	[Required]
	public Guid CreatedUserId { get; init; }
	[Required]
	public string CreatedUsername { get; init; } = default!;
	[Required]
	public string Remark { get; init; } = default!;
}
