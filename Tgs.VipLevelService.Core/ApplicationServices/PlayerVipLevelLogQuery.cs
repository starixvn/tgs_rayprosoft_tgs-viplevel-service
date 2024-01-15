using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelLogQuery(
	string PlayerCode,
	DateTimeOffset StartOn,
	DateTimeOffset EndOn,
	int Page = 1,
	int RowCount = 20);
