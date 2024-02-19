using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelExpAddDepositCommand(
	string PlayerCode,
	string FormId,
	decimal Amount);
