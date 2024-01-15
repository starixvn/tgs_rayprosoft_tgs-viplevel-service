using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLeveRegisterViewModel
{
	public Guid? VipLevelId { get; init; }
	public int? VipLevel { get; init; }
	[Required]
	public PlayerVipLevelAutoState AutoState { get; init; }
}
