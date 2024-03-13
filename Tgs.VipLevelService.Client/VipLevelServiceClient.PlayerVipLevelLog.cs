using System.Net.Http.Json;
using System.Web;

namespace Tgs.VipLevelService.Clients;
internal partial class VipLevelServiceClient
{
	public IAsyncEnumerable<Common.ViewModels.PlayerVipLevelLogQueryViewModel> PlayerVipLevelLogQueryAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, int page, int rowCount, CancellationToken cancellationToken = default)

	{
		var path = new Uri(_httpClient.BaseAddress!, $"player-log/{playerCode}");
		var builder = new UriBuilder(path);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["startOn"] = startOn.ToString("u");
		query["endOn"] = endOn.ToString("u");
		query["page"] = page.ToString();
		query["rowCount"] = rowCount.ToString();
		builder.Query = query.ToString();
		string url = builder.ToString();

		return _httpClient.GetAsync(url, cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<IEnumerable<Common.ViewModels.PlayerVipLevelLogQueryViewModel>>(_SerializerOptions, cancellationToken)
				.ToAsyncEnumerable()
				.SelectMany(arr => arr!.ToAsyncEnumerable()))
			.Where(info => info is not null)
			.Select(info => info!)
			.ToEnumerable()
			.ToAsyncEnumerable();
	}
	public async ValueTask<int> PlayerVipLevelLogQueryCountAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, CancellationToken cancellationToken = default)
	{
		var path = new Uri(_httpClient.BaseAddress!, $"player-log/{playerCode}/count");
		var builder = new UriBuilder(path);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["startOn"] = startOn.ToString("u");
		query["endOn"] = endOn.ToString("u");
		builder.Query = query.ToString();
		string url = builder.ToString();

		return await _httpClient.GetAsync(url, cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<int>(_SerializerOptions, cancellationToken)
				.ToAsyncEnumerable())
			.FirstOrDefaultAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false);
	}
}
