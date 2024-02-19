using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.VipLevelService.ApplicationServices;
public record PlayerVipLevelExpAddTurnoverCommand(
	string PlayerCode,
	Guid GameProviderId,
	int GameId,
	Guid WalletId,
	string ExternalWagerId,
	long WagerId,
	decimal Amount);
