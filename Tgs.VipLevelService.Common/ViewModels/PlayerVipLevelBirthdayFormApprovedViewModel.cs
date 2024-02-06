using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.VipLevelService.Common.ViewModels;
public record PlayerVipLevelBirthdayFormApprovedViewModel
{
	[Required()]
	[Range(minimum: 0, maximum: int.MaxValue)]
	public decimal Amount { get; init; }

	public string Remark { get; init; } = default!;
}
