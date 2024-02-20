using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.VipLevelService;
public record VipLevelPolicySettingsOptions
{
	public int PlayerVipLevelExpDepositRetryCount { get; set; } = 1;
	public int PlayerVipLevelExpDepositRetryAttemptSec { get; set; } = 5;
	public int PlayerVipLevelExpTurnoverRetryCount { get; set; } = 1;
	public int PlayerVipLevelExpTurnoverRetryAttemptSec { get; set; } = 5;
}
