using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.ViewModels;

public record PlayerVipLevelLogFromQueryCount
{
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset StartOn { get; init; }

	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset EndOn { get; init; }
}
