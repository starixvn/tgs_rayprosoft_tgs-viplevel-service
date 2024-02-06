using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelBirthdayFormCreateViewModel
{
	[Required]
	public string PlayerCode { get; init; } = default!;
	[Required]
	public DateOnly Birthday { get; init; }
}
