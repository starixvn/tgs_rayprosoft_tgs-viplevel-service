using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.ViewModels;

public record PlayerVipLevelUpgradeFormFromQueryCount
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
}
