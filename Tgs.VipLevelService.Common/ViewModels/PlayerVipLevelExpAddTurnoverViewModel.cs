using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelExpAddTurnoverViewModel
{
	[Required]
	public Guid GameProviderId { get; init; }
	[Required]
	public int GameId { get; init; } = default!;
	[Required]
	public Guid WalletId { get; init; }
	[Required]
	public string ExternalWagerId { get; init; } = default!;
	[Required]
	public long WagerId { get; init; }
	[Required]
	[Range(minimum: 0, maximum: int.MaxValue)]
	public decimal Amount { get; init; }
}
