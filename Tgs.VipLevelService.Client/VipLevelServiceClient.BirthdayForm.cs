using System.Net.Http.Json;
using System.Web;
using Microsoft.AspNetCore.Http;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Common.ViewModels;

namespace Tgs.VipLevelService.Clients;

internal partial class VipLevelServiceClient
{
	public async ValueTask<Guid> PlayerVipLevelBirthdayFormCreateAsync(string playerCode, DateOnly birthday, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelBirthdayFormCreateViewModel
		{
			PlayerCode = playerCode,
			Birthday = birthday
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PostAsync($"/birthday-form", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<Guid>(
			_SerializerOptions,
			cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask PlayerVipLevelBirthdayFormPendingAsync(Guid id, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(new { }, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PatchAsync($"/birthday-form/{id}/pending", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask PlayerVipLevelBirthdayFormApprovedAsync(Guid id, decimal amount, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelBirthdayFormApprovedViewModel
		{
			Amount = amount,
			Remark = remark
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PatchAsync($"/birthday-form/{id}/approved", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask PlayerVipLevelBirthdayFormRejectAsync(Guid id, string remark, Guid operatorUserId, string operatorUsername, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelBirthdayFormRejectViewModel
		{
			Remark = remark
		};
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("OperatorUserId", operatorUserId.ToString());
		requestContent.Headers.Add("OperatorUsername", operatorUsername);

		var response = await _httpClient
			.PatchAsync($"/birthday-form/{id}/reject", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask<bool> PlayerVipLevelBirthdayFormExistAsync(string playerCode, DateOnly birthday, CancellationToken cancellationToken = default)
		=> await _httpClient.GetAsync($"/birthday-form/exist/{playerCode}/{birthday:yyyy-MM-dd}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<bool>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public async ValueTask<PlayerVipLevelBirthdayFormViewModel?> PlayerVipLevelBirthdayFormGetAsync(Guid id, CancellationToken cancellationToken = default)
		=> await _httpClient.GetAsync($"/birthday-form/{id}", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<PlayerVipLevelBirthdayFormViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false);

	public IAsyncEnumerable<PlayerVipLevelBirthdayFormQueryViewModel?> PlayerVipLevelBirthdayFormQueryAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<PlayerVipLevelBirthdayFormState> states, int page = 1, int rowCount = 20, CancellationToken cancellationToken = default)
	{
		var path = new Uri(_httpClient.BaseAddress!, "birthday-form");
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
				.ReadFromJsonAsync<IEnumerable<PlayerVipLevelBirthdayFormQueryViewModel>>(_SerializerOptions, cancellationToken)
				.ToAsyncEnumerable()
				.SelectMany(arr => arr!.ToAsyncEnumerable()))
			.Where(info => info is not null)
			.Select(info => info!)
			.ToEnumerable()
			.ToAsyncEnumerable();
	}

	public async ValueTask<int> PlayerVipLevelBirthdayFormQueryCountAsync(Guid brandId, DateTimeOffset updatedOnStart, DateTimeOffset updatedOnEnd, string playerCode, string username, IEnumerable<PlayerVipLevelBirthdayFormState> states, CancellationToken cancellationToken = default)
	{
		var path = new Uri(_httpClient.BaseAddress!, $"/birthday-form/count");
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
