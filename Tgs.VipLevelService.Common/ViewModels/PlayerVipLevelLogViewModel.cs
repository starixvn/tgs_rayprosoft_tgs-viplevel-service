using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;

public class PlayerVipLevelLogViewModel
{
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset StartOn { get; init; }

	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset EndOn { get; init; }

	[Required]
	[Range(minimum: 1, maximum: int.MaxValue)]
	public int Page { get; init; } = 1;

	[Required]
	[Range(minimum: 1, maximum: int.MaxValue)]
	public int RowCount { get; init; } = 20;
}
