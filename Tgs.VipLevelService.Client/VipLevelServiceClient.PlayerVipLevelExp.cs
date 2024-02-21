using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Nodes;
using Tgs.VipLevelService.Common.ViewModels;

namespace Tgs.VipLevelService.Clients;

internal partial class VipLevelServiceClient
{
	public async ValueTask PlayerVipLevelExpAddTurnoverAsync(string playerCode, Guid gameProviderId, int gameId, Guid walletId, string externalWagerId, long wagerId, decimal amount, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelExpAddTurnoverViewModel
		{
			GameProviderId = gameProviderId,
			GameId = gameId,
			WalletId = walletId,
			ExternalWagerId = externalWagerId,
			WagerId = wagerId,
			Amount = amount
		};
		var path = new Uri(_httpClient.BaseAddress!, $"exp/{playerCode}/turnover");
		var builder = new UriBuilder(path);
		string url = builder.ToString();
		var jsonContent = JsonContent.Create(source, options: _SerializerOptions);

		_ = await _httpClient.PostAsync(url, jsonContent, cancellationToken)
			.ToAsyncEnumerable()
		   .Do(response => response.EnsureSuccessStatusCode())
		   .FirstAsync(cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
	public async ValueTask PlayerVipLevelExpAddTurnoverCreditAsync(string playerCode, Guid formId, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelExpAddTurnoverManualViewModel
		{
			FormId = formId,
			Amount = amount,
			Remark = remark
		};
		var path = new Uri(_httpClient.BaseAddress!, $"exp/{playerCode}/turnover/credit");
		var builder = new UriBuilder(path);
		string url = builder.ToString();
		var jsonContent = JsonContent.Create(source, options: _SerializerOptions);
		jsonContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		jsonContent.Headers.Add("OperatorUsername", operatorUsername);

		_ = await _httpClient.PostAsync(url, jsonContent, cancellationToken)
			.ToAsyncEnumerable()
		   .Do(response => response.EnsureSuccessStatusCode())
		   .FirstAsync(cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
	public async ValueTask PlayerVipLevelExpAddTurnoverDebitAsync(string playerCode, Guid formId, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelExpAddTurnoverManualViewModel
		{
			FormId = formId,
			Amount = amount,
			Remark = remark
		};
		var path = new Uri(_httpClient.BaseAddress!, $"exp/{playerCode}/turnover/debit");
		var builder = new UriBuilder(path);
		string url = builder.ToString();
		var jsonContent = JsonContent.Create(source, options: _SerializerOptions);
		jsonContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		jsonContent.Headers.Add("OperatorUsername", operatorUsername);

		_ = await _httpClient.PostAsync(url, jsonContent, cancellationToken)
			.ToAsyncEnumerable()
		   .Do(response => response.EnsureSuccessStatusCode())
		   .FirstAsync(cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
	public async ValueTask PlayerVipLevelExpAddDepositAsync(string playerCode, string formId, decimal amount, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelExpAddDepositViewModel
		{
			FormId = formId,
			Amount = amount
		};
		var path = new Uri(_httpClient.BaseAddress!, $"exp/{playerCode}/deposit");
		var builder = new UriBuilder(path);
		string url = builder.ToString();
		var jsonContent = JsonContent.Create(source, options: _SerializerOptions);

		_ = await _httpClient.PostAsync(url, jsonContent, cancellationToken)
			.ToAsyncEnumerable()
		   .Do(response => response.EnsureSuccessStatusCode())
		   .FirstAsync(cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
	public async ValueTask PlayerVipLevelExpAddDepositCreditAsync(string playerCode, Guid formId, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelExpAddDepositManualViewModel
		{
			FormId = formId,
			Amount = amount,
			Remark = remark
		};
		var path = new Uri(_httpClient.BaseAddress!, $"exp/{playerCode}/deposit/credit");
		var builder = new UriBuilder(path);
		string url = builder.ToString();
		var jsonContent = JsonContent.Create(source, options: _SerializerOptions);
		jsonContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		jsonContent.Headers.Add("OperatorUsername", operatorUsername);

		_ = await _httpClient.PostAsync(url, jsonContent, cancellationToken)
			.ToAsyncEnumerable()
		   .Do(response => response.EnsureSuccessStatusCode())
		   .FirstAsync(cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
	public async ValueTask PlayerVipLevelExpAddDepositDebitAsync(string playerCode, Guid formId, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelExpAddDepositManualViewModel
		{
			FormId = formId,
			Amount = amount,
			Remark = remark
		};
		var path = new Uri(_httpClient.BaseAddress!, $"exp/{playerCode}/deposit/debit");
		var builder = new UriBuilder(path);
		string url = builder.ToString();
		var jsonContent = JsonContent.Create(source, options: _SerializerOptions);
		jsonContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		jsonContent.Headers.Add("OperatorUsername", operatorUsername);

		_ = await _httpClient.PostAsync(url, jsonContent, cancellationToken)
			.ToAsyncEnumerable()
		   .Do(response => response.EnsureSuccessStatusCode())
		   .FirstAsync(cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}
}
