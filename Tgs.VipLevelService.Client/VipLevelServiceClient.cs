using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Json.Patch;

namespace Tgs.VipLevelService.Clients;

internal class VipLevelServiceClient(HttpClient httpClient) : IVipLevelServiceClient
{
	private static readonly JsonSerializerOptions _SerializerOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};

	public IAsyncEnumerable<VipLevelInfo> GetBrandAllVipLevelsAsync(Guid brandId, CancellationToken cancellationToken = default)
		=> httpClient
			.GetAsync($"/vip/{brandId}", cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<IEnumerable<VipLevelInfo>>(_SerializerOptions, cancellationToken)
				.ToAsyncEnumerable()
				.Where(collection => collection is not null)
				.SelectMany(collection => collection!.ToAsyncEnumerable()))
			.ToArrayAsync(cancellationToken)
			.AsTask()
			.ToAsyncEnumerable()
			.SelectMany(arr => arr.ToAsyncEnumerable());

	public async ValueTask<Guid> CreateVipLevelAsync(
		Guid brandId,
		VipLevelCreateInfo createInfo,
		string modifier,
		CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(createInfo, options: _SerializerOptions);
		requestContent.Headers.Add("Modifier", modifier);

		var response = await httpClient.PutAsync(
			$"/vip/{brandId}",
			requestContent,
			cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<Guid>(
			_SerializerOptions,
			cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask UpdateVipLevelAsync(
		Guid vipId,
		VipLevelUpdateInfo updateInfo,
		string modifier,
		CancellationToken cancellationToken = default)
	{
		var patch = VipLevelUpdateInfo.Default.CreatePatch(updateInfo, _SerializerOptions);

		var requestContent = JsonContent.Create(patch, options: _SerializerOptions);
		requestContent.Headers.Add("Modifier", modifier);

		var response = await httpClient
			.PatchAsync($"/vip/{vipId}", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask<PagingResponse<PlayerVipAdjustInfo>> QueryPlayerVipAdjustInfosAsync(
		PlayerVipAdjustQueryConditions conditions,
		Paging paging,
		CancellationToken cancellationToken = default)
	{
		var queryStrings = HttpUtility.ParseQueryString(string.Empty);

		queryStrings.Add("brand_id", conditions.BrandId.ToString());
		queryStrings.Add("player_code", conditions.PlayerCode);

		if (conditions.CreateOnRange != null)
		{
			queryStrings.Add("bc", conditions.CreateOnRange.Value.Begin.ToString("o"));
			queryStrings.Add("ec", conditions.CreateOnRange.Value.End.ToString("o"));
		}

		if (paging.ShowAll == true)
			queryStrings.Add("all", "true");
		else
		{
			if (paging.Page != null)
				queryStrings.Add("page", paging.Page.ToString());

			if (paging.Rows != null)
				queryStrings.Add("rows", paging.Rows.ToString());
		}

		if (!string.IsNullOrWhiteSpace(paging.SortBy))
		{
			queryStrings.Add("sort", paging.SortBy);

			if (paging.IsDesc == true)
				queryStrings.Add("desc", "true");
		}

		var response = await httpClient
			.GetAsync($"/vip/playerAdjustInfo?{queryStrings}", cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		var contentJson = await response.Content
			.ReadAsStringAsync(cancellationToken)
			.ConfigureAwait(false);

		return JsonSerializer.Deserialize<PagingResponse<PlayerVipAdjustInfo>>(contentJson, _SerializerOptions)!;
	}
}
