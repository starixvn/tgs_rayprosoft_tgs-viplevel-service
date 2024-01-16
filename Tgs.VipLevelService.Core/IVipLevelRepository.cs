using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService;

public interface IVipLevelRepository
{
	IAsyncEnumerable<VipLevelInfo> QueryAsync(Guid brandId, CancellationToken cancellationToken = default);

	ValueTask<VipLevelInfo?> GetAsync(Guid brandId, Guid id, CancellationToken cancellationToken = default);

	ValueTask<VipLevelInfo?> GetAsync(Guid brandId, int level, CancellationToken cancellationToken = default);

	ValueTask<VipLevelInfo?> GetAsync(Guid id, CancellationToken cancellationToken = default);

	ValueTask UpdateAsync(Guid id, Guid brandId, VipLevelUpdate update, string modifier, CancellationToken cancellationToken = default);

	ValueTask CreateAsync(Guid id, Guid brandId, VipLevelCreate create, string modifier, CancellationToken cancellationToken = default);
}
