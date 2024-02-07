using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Common.ViewModels;

namespace Tgs.VipLevelService.Clients;

internal partial class VipLevelServiceClient
{
	public async ValueTask<Guid> PlayerVipLevelCycleFormCreateAsync(string playerCode, PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn, DateOnly fromOn, DateOnly toOn, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelCycleFormCreateViewModel
		{
			PlayerCode = playerCode,
			CycleType = cycleType,
			CycleOn = cycleOn,
			FromOn = fromOn,
			ToOn = toOn
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PostAsync($"/cycle-form", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<Guid>(
			_SerializerOptions,
			cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask PlayerVipLevelCycleFormPendingAsync(Guid id, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(new { }, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PatchAsync($"/cycle-form/{id}/pending", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask PlayerVipLevelCycleFormApprovedAsync(Guid id, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelCycleFormApprovedViewModel
		{
			Amount = amount,
			Remark = remark
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PatchAsync($"/cycle-form/{id}/approved", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask PlayerVipLevelCycleFormRejectAsync(Guid id, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelCycleFormRejectViewModel
		{
			Remark = remark
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PatchAsync($"/cycle-form/{id}/reject", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask<bool> PlayerVipLevelCycleFormExistAsync(string playerCode, PlayerVipLevelCycleFormCycleType cycleType, DateOnly cycleOn, CancellationToken cancellationToken = default)
		=> await _httpClient.GetAsync($"/cycle-form/exist/{playerCode}/{cycleType}/{cycleOn:yyyy-MM-dd}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<bool>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<PlayerVipLevelCycleFormViewModel?> PlayerVipLevelCycleFormGetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await _httpClient.GetAsync($"/cycle-form/{id}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<PlayerVipLevelCycleFormViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);
}
