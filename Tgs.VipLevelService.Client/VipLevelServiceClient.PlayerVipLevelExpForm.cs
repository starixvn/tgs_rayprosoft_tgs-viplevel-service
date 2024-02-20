using System.Net.Http.Json;
using Tgs.VipLevelService.Common.ViewModels;

namespace Tgs.VipLevelService.Clients;

internal partial class VipLevelServiceClient
{
	public async ValueTask<PlayerVipLevelExpFormViewModel?> PlayerVipLevelExpFormGetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await _httpClient.GetAsync($"/exp-form/{id}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<PlayerVipLevelExpFormViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<Guid> PlayerVipLevelExpFormCreateDepositCreditAsync(string playerCode, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelExpFormCreateViewModel
		{
			PlayerCode = playerCode,
			Amount = amount,
			Remark = remark
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PostAsync($"/exp-form/deposit/credit", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<Guid>(
			_SerializerOptions,
			cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask<Guid> PlayerVipLevelExpFormCreateDepositDebitAsync(string playerCode, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelExpFormCreateViewModel
		{
			PlayerCode = playerCode,
			Amount = amount,
			Remark = remark
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PostAsync($"/exp-form/deposit/debit", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<Guid>(
			_SerializerOptions,
			cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask<Guid> PlayerVipLevelExpFormCreateTurnoverCreditAsync(string playerCode, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelExpFormCreateViewModel
		{
			PlayerCode = playerCode,
			Amount = amount,
			Remark = remark
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PostAsync($"/exp-form/turnover/credit", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<Guid>(
			_SerializerOptions,
			cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask<Guid> PlayerVipLevelExpFormCreateTurnoverDebitAsync(string playerCode, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelExpFormCreateViewModel
		{
			PlayerCode = playerCode,
			Amount = amount,
			Remark = remark
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PostAsync($"/exp-form/turnover/debit", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<Guid>(
			_SerializerOptions,
			cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask PlayerVipLevelExpFormPendingAsync(Guid id, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(new { }, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PatchAsync($"/exp-form/{id}/pending", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask PlayerVipLevelExpFormApprovedAsync(Guid id, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelExpFormApprovedViewModel
		{
			Amount = amount,
			Remark = remark
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PatchAsync($"/exp-form/{id}/approved", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask PlayerVipLevelExpFormRejectAsync(Guid id, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelExpFormRejectViewModel
		{
			Remark = remark
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PatchAsync($"/exp-form/{id}/reject", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}
}
