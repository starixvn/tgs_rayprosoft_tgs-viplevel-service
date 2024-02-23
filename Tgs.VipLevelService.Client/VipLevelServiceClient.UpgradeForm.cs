using System.Net.Http.Json;
using System.Web;

namespace Tgs.VipLevelService.Clients;

internal partial class VipLevelServiceClient
{
	public async ValueTask<Guid> PlayerVipLevelUpgradeFormCreateAsync(string playerCode, int vipLevel, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new Common.ViewModels.PlayerVipLevelUpgradeFormCreateViewModel
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
		var source = new Common.ViewModels.PlayerVipLevelUpgradeFormApprovedViewModel
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
		var source = new Common.ViewModels.PlayerVipLevelUpgradeFormRejectViewModel
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

	public async ValueTask<Common.ViewModels.PlayerVipLevelUpgradeFormViewModel?> PlayerVipLevelUpgradeFormGetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await _httpClient.GetAsync($"/upgrade-form/{id}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<Common.ViewModels.PlayerVipLevelUpgradeFormViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public IAsyncEnumerable<Common.ViewModels.PlayerVipLevelUpgradeFormQueryViewModel?> PlayerVipLevelUpgradeFormQueryAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<Common.PlayerVipLevelUpgradeFormState> states, int page = 1, int rowCount = 20, CancellationToken cancellationToken = default)
	{
		var path = new Uri(_httpClient.BaseAddress!, "upgrade-form");
		var builder = new UriBuilder(path);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["brandId"] = brandId.ToString();
		query["updatedOnStart"] = updatedOnStart.ToString("u");
		query["updatedOnEnd"] = updatedOnEnd.ToString("u");

		if (!string.IsNullOrWhiteSpace(playerCode))
		{
			query["playerCode"] = playerCode;
		}

		if (!string.IsNullOrWhiteSpace(username))
		{
			query["username"] = username;
		}

		if (states.Any())
		{
			foreach (var state in states)
			{
				query.Add("states", state.ToString());
			}
		}

		query["page"] = page.ToString();
		query["rowCount"] = rowCount.ToString();
		builder.Query = query.ToString();
		string url = builder.ToString();

		return _httpClient.GetAsync(url, cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<IEnumerable<Common.ViewModels.PlayerVipLevelUpgradeFormQueryViewModel>>(_SerializerOptions, cancellationToken)
				.ToAsyncEnumerable()
				.SelectMany(arr => arr!.ToAsyncEnumerable()))
			.Where(info => info is not null)
			.Select(info => info!)
			.ToEnumerable()
			.ToAsyncEnumerable();
	}

	public async ValueTask<int> PlayerVipLevelUpgradeFormQueryCountAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<Common.PlayerVipLevelUpgradeFormState> states, CancellationToken cancellationToken = default)
	{
		var path = new Uri(_httpClient.BaseAddress!, $"/upgrade-form/count");
		var builder = new UriBuilder(path);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["brandId"] = brandId.ToString();
		query["updatedOnStart"] = updatedOnStart.ToString("u");
		query["updatedOnEnd"] = updatedOnEnd.ToString("u");

		if (!string.IsNullOrWhiteSpace(playerCode))
		{
			query["PlayerCode"] = playerCode;
		}

		if (!string.IsNullOrWhiteSpace(username))
		{
			query["Username"] = username;
		}

		if (states.Any())
		{
			foreach (var state in states)
			{
				query.Add("status", state.ToString());
			}
		}

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
