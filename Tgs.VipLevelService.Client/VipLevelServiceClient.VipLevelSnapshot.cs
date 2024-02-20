using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Tgs.VipLevelService.Common.ViewModels;

namespace Tgs.VipLevelService.Clients;

internal partial class VipLevelServiceClient
{
	public IAsyncEnumerable<VipLevelInfoViewModel> VipLevelSnapshotQueryVipLevelAsync(Guid id, CancellationToken cancellationToken = default)
		=> _httpClient
			.GetAsync($"/vip-snap/{id}", cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<IEnumerable<VipLevelInfoViewModel>>(_SerializerOptions, cancellationToken)
				.ToAsyncEnumerable()
				.Where(collection => collection is not null)
				.SelectMany(collection => collection!.ToAsyncEnumerable()))
			.ToArrayAsync(cancellationToken)
			.AsTask()
			.ToAsyncEnumerable()
			.SelectMany(arr => arr.ToAsyncEnumerable());

	public async ValueTask<Guid> VipLevelSnapshotCurrentSnapshotIdAsync(Guid brandId, CancellationToken cancellationToken = default)
	{
		var response = await _httpClient.GetAsync(
			$"/vip-snap/{brandId}/current/snapshot-id",
			cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<Guid>(
			_SerializerOptions,
			cancellationToken).ConfigureAwait(false);
	}
}
