using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.ViewModels;

public record PlayerVipLevelUpgradeFormFromQuery
{
	[Required]
	public Guid BrandId { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset UpdatedOnStart { get; init; }
	[Required]
	[DataType(DataType.DateTime)]
	public DateTimeOffset UpdatedOnEnd { get; init; }
	public string? Username { get; init; }
	public string? PlayerCode { get; init; }
	public Common.PlayerVipLevelUpgradeFormState[]? States { get; init; }
	[Required]
	[Range(minimum: 1, maximum: int.MaxValue)]
	public int Page { get; init; } = 1;

	[Required]
	[Range(minimum: 1, maximum: int.MaxValue)]
	public int RowCount { get; init; } = 20;
}
