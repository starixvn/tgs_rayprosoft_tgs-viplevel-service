using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.VipLevelService.Core.Protos;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelPlayerCodeResult(
	Guid ResellerId,
	string ResellerCode,
	Guid LicenseeId,
	string LicenseeCode,
	Guid BrandId,
	string BrandCode,
	string PlayerCode,
	string Username,
	Guid VipLevelId,
	int VipLevel,
	PlayerVipLevelAutoState AutoState,
	Guid AccumDepositId,
	decimal AccumDeposit,
	Guid AccumTurnoverId,
	decimal AccumTurnover);
