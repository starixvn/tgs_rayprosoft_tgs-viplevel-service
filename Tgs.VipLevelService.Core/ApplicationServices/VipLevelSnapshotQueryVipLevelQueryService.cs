using System.Runtime.CompilerServices;
using System.Text.Json;
using Tgs.ServiceUtility;
using Tgs.VipLevelService.Models;

namespace Tgs.VipLevelService.ApplicationServices;

internal class VipLevelSnapshotQueryVipLevelQueryService(
	IVipLevelSnapshotRepository repository) : IQueryService<VipLevelSnapshotQueryVipLevelQuery, VipLevelInfo>
{
	public async IAsyncEnumerable<VipLevelInfo> QueryAsync(VipLevelSnapshotQueryVipLevelQuery query, [EnumeratorCancellation] CancellationToken cancellationToken = default)
	{
		var vipLevelSnapshotInfo = await repository.GetAsync(
			query.Id,
			cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException($"This VipLevel snapshot does not exist.({query.Id})");

		var vipLevelInfos = JsonSerializer.Deserialize<IEnumerable<VipLevelInfo>>(vipLevelSnapshotInfo.Data)
			?? Enumerable.Empty<VipLevelInfo>();

		foreach (var vipLevelInfo in vipLevelInfos)
		{
			yield return vipLevelInfo;
		}
	}
}
