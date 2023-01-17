namespace Tgs.VipLevelService.Clients;

public class PlayerVipAdjustQueryConditions
{
	public Guid BrandId { get; init; }

	public string PlayerCode { get; init; } = default!;
}
