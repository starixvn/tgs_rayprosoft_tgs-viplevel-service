using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.ViewModels;

public record PlayerVipLevelCycleFormFromQueryCount
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
	public Common.PlayerVipLevelCycleFormState[]? States { get; init; }
}
