using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;

namespace Tgs.VipLevelService.Clients;

internal partial class VipLevelServiceClient
{
	public async ValueTask PlayerVipLevelExpAddTurnoverAsync(string playerCode, Guid gameProviderId, int gameId, Guid walletId, string externalWagerId, long wagerId, decimal amount, CancellationToken cancellationToken = default)
	{
		var source = new JsonObject
		{
			["gameProviderId"] = gameProviderId,
			["gameId"] = gameId,
			["walletId"] = walletId,
			["externalWagerId"] = externalWagerId,
			["wagerId"] = wagerId,
			["amount"] = amount
		};
		var path = new Uri(_httpClient.BaseAddress!, $"player-exp/{playerCode}/turnover");
		var builder = new UriBuilder(path);
		string url = builder.ToString();
		var jsonContent = JsonContent.Create(source, options: _SerializerOptions);

		_ = await _httpClient.PostAsync(url, jsonContent, cancellationToken)
			.ToAsyncEnumerable()
		   .Do(response => response.EnsureSuccessStatusCode())
		   .FirstAsync(cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
	public async ValueTask PlayerVipLevelExpAddDepositAsync(string playerCode, string formId, decimal amount, CancellationToken cancellationToken = default)
	{
		var source = new JsonObject
		{
			["formId"] = formId,
			["amount"] = amount
		};
		var path = new Uri(_httpClient.BaseAddress!, $"player-exp/{playerCode}/deposit");
		var builder = new UriBuilder(path);
		string url = builder.ToString();
		var jsonContent = JsonContent.Create(source, options: _SerializerOptions);

		_ = await _httpClient.PostAsync(url, jsonContent, cancellationToken)
			.ToAsyncEnumerable()
		   .Do(response => response.EnsureSuccessStatusCode())
		   .FirstAsync(cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
}
