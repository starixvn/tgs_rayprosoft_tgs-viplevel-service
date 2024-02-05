using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tgs.VipLevelService;
public interface IPlayerVipLevelUpgradePromotionRepository
{
	ValueTask AddAsync(Guid id, Guid snapshotId, Guid vipLevelId, string playerCode, decimal amount, CancellationToken cancellationToken = default);
	ValueTask<bool> ExistAsync(Guid vipLevelId, string playerCode, CancellationToken cancellationToken = default);
}
