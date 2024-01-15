using System.ComponentModel.DataAnnotations;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelRegisterViewModel
{
	public Guid? VipLevelId { get; init; }
	public int? VipLevel { get; init; }
	[Required]
	public PlayerVipLevelAutoState AutoState { get; init; }
}
