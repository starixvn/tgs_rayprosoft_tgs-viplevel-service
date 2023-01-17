using Microsoft.AspNetCore.Mvc;

namespace Tgs.VipLevelService.ViewModels;

public class PlayerVipAdjustQueryViewModel
{
	[FromQuery(Name = "brand_id")]
	public Guid BrandId { get; set; }

	[FromQuery(Name = "player_code")]
	public string PlayerCode { get; set; } = default!;
}
