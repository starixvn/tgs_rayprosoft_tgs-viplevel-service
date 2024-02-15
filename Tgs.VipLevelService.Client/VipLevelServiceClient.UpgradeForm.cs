using System.Net.Http.Json;
using Tgs.VipLevelService.Common.ViewModels;

namespace Tgs.VipLevelService.Clients;

internal partial class VipLevelServiceClient
{
	public async ValueTask<Guid> PlayerVipLevelUpgradeFormCreateAsync(string playerCode, int vipLevel, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelUpgradeFormCreateViewModel
		{
			PlayerCode = playerCode,
			VipLevel = vipLevel
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PostAsync($"/upgrade-form", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<Guid>(
			_SerializerOptions,
			cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask PlayerVipLevelUpgradeFormPendingAsync(Guid id, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(new { }, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PatchAsync($"/upgrade-form/{id}/pending", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask PlayerVipLevelUpgradeFormApprovedAsync(Guid id, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelUpgradeFormApprovedViewModel
		{
			Amount = amount,
			Remark = remark
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PatchAsync($"/upgrade-form/{id}/approved", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask PlayerVipLevelUpgradeFormRejectAsync(Guid id, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelUpgradeFormRejectViewModel
		{
			Remark = remark
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PatchAsync($"/upgrade-form/{id}/reject", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask<bool> PlayerVipLevelUpgradeFormExistAsync(string playerCode, int vipLevel, CancellationToken cancellationToken = default)
		=> await _httpClient.GetAsync($"/upgrade-form/exist/{playerCode}/{vipLevel}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<bool>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<PlayerVipLevelUpgradeFormViewModel?> PlayerVipLevelUpgradeFormGetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await _httpClient.GetAsync($"/upgrade-form/{id}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<PlayerVipLevelUpgradeFormViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
