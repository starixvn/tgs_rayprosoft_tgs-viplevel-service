using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Mime;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Json.Patch;
using Tgs.VipLevelService.Common;
using Tgs.VipLevelService.Common.ViewModels;

namespace Tgs.VipLevelService.Clients;

internal class VipLevelServiceClient : IVipLevelServiceClient
{
	private static readonly JsonSerializerOptions _SerializerOptions = new()
	{
		WriteIndented = false,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		Converters = { new JsonStringEnumConverter() }
	};

	private readonly HttpClient _httpClient;

	public VipLevelServiceClient(HttpClient httpClient)
	{
		_httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
	}

	public IAsyncEnumerable<VipLevelInfoViewModel> GetBrandAllVipLevelsAsync(Guid brandId, CancellationToken cancellationToken = default)
		=> _httpClient
			.GetAsync($"/vip/{brandId}", cancellationToken)
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

	public async ValueTask<Guid> CreateVipLevelAsync(Guid brandId, VipLevelCreateViewModel source, string modifier, CancellationToken cancellationToken = default)
	{
		var requestContent = JsonContent.Create(source, options: _SerializerOptions);
		requestContent.Headers.Add("Modifier", modifier);

		var response = await _httpClient.PutAsync(
			$"/vip/{brandId}",
			requestContent,
			cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<Guid>(
			_SerializerOptions,
			cancellationToken).ConfigureAwait(false);
	}

	public async ValueTask UpdateVipLevelAsync(Guid vipId, VipLevelUpdateViewModel source, string modifier, CancellationToken cancellationToken = default)
	{
		var patch = VipLevelUpdateViewModel.Default.CreatePatch(source, _SerializerOptions);

		var requestContent = JsonContent.Create(patch, options: _SerializerOptions);
		requestContent.Headers.Add("Modifier", modifier);

		var response = await _httpClient
			.PatchAsync($"/vip/{vipId}", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}

	public async ValueTask<PagingResponseViewModel<PlayerVipAdjustInfo>> QueryPlayerVipAdjustInfosAsync(PlayerVipAdjustQueryConditions conditions, Paging paging, CancellationToken cancellationToken = default)
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

		var response = await _httpClient
			.GetAsync($"/vip/playerAdjustInfo?{queryStrings}", cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		var contentJson = await response.Content
			.ReadAsStringAsync(cancellationToken)
			.ConfigureAwait(false);

		return JsonSerializer.Deserialize<PagingResponseViewModel<PlayerVipAdjustInfo>>(contentJson, _SerializerOptions)!;
	}

	public async ValueTask<PlayerVipLevelInfoViewModel> PlayerVipLevelGetAsync(string playerCode, CancellationToken cancellationToken = default)
		=> await _httpClient.GetAsync($"/player/{playerCode}/viplevel", cancellationToken)
		.ToAsyncEnumerable()
		.Do(response => response.EnsureSuccessStatusCode())
		.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
		.SelectMany(response => response.Content
			.ReadFromJsonAsync<PlayerVipLevelInfoViewModel>(_SerializerOptions, cancellationToken)
			.ToAsyncEnumerable())
		.FirstOrDefaultAsync(cancellationToken: cancellationToken)
		.ConfigureAwait(false)
		?? throw new NullReferenceException($"This player's VIP Level does not exist.{playerCode}");

	public async ValueTask PlayerVipLevelRegisterAsync(string playerCode, int? vipLevel, PlayerVipLevelAutoState autoState, Guid? vipLevelId = null, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelRegisterViewModel
		{
			VipLevel = vipLevel,
			VipLevelId = vipLevelId,
			AutoState = autoState
		};
		var path = new Uri(_httpClient.BaseAddress!, $"player/{playerCode}/viplevel/register");
		var builder = new UriBuilder(path);
		string url = builder.ToString();
		var httpContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

		_ = await _httpClient.PostAsync(url, httpContent, cancellationToken)
			.ToAsyncEnumerable()
		   .Do(response => response.EnsureSuccessStatusCode())
		   .FirstAsync(cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}

	public async ValueTask PlayerVipLevelManualAsync(string playerCode, int? vipLevel, PlayerVipLevelAutoState autoState, Guid createdUserId, string createdUsername, string remark, Guid? vipLevelId = null, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelManualViewModel
		{
			VipLevel = vipLevel,
			VipLevelId = vipLevelId,
			AutoState = autoState,
			CreatedUserId = createdUserId,
			CreatedUsername = createdUsername,
			Remark = remark
		};
		var path = new Uri(_httpClient.BaseAddress!, $"player/{playerCode}/viplevel/manual");
		var builder = new UriBuilder(path);
		string url = builder.ToString();
		var httpContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

		_ = await _httpClient.PostAsync(url, httpContent, cancellationToken)
			.ToAsyncEnumerable()
		   .Do(response => response.EnsureSuccessStatusCode())
		   .FirstAsync(cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}

	public async ValueTask PlayerVipLevelSystemAsync(string playerCode, int? vipLevel, Guid? vipLevelId = null, CancellationToken cancellationToken = default)
	{
		var source = new PlayerVipLevelSystemViewModel
		{
			VipLevel = vipLevel,
			VipLevelId = vipLevelId
		};
		var path = new Uri(_httpClient.BaseAddress!, $"player/{playerCode}/viplevel/system");
		var builder = new UriBuilder(path);
		string url = builder.ToString();
		var httpContent = new StringContent(JsonSerializer.Serialize(source, _SerializerOptions), Encoding.UTF8, MediaTypeNames.Application.Json);

		_ = await _httpClient.PostAsync(url, httpContent, cancellationToken)
			.ToAsyncEnumerable()
		   .Do(response => response.EnsureSuccessStatusCode())
		   .FirstAsync(cancellationToken: cancellationToken)
		   .ConfigureAwait(false);
	}

	public async ValueTask<PagingResponseViewModel<PlayerVipLevelLogInfoViewModel>> PlayerVipLevelLogQueryAsync(string playerCode, DateTimeOffset startOn, DateTimeOffset endOn, int page, int rowCount, CancellationToken cancellationToken = default)
	{
		var path = new Uri(_httpClient.BaseAddress!, $"player/{playerCode}/viplevel/log");
		var builder = new UriBuilder(path);
		var query = HttpUtility.ParseQueryString(builder.Query);
		query["startOn"] = startOn.ToString("u");
		query["endOn"] = endOn.ToString("u");
		query["page"] = page.ToString();
		query["rowCount"] = rowCount.ToString();
		builder.Query = query.ToString();
		string url = builder.ToString();

		return await _httpClient.GetAsync(url, cancellationToken)
			.ToAsyncEnumerable()
			.Do(response => response.EnsureSuccessStatusCode())
			.Where(response => response.StatusCode == System.Net.HttpStatusCode.OK)
			.SelectMany(response => response.Content
				.ReadFromJsonAsync<PagingResponseViewModel<PlayerVipLevelLogInfoViewModel>>(_SerializerOptions, cancellationToken)
				.ToAsyncEnumerable())
			.FirstOrDefaultAsync(cancellationToken: cancellationToken)
			.ConfigureAwait(false)
			?? throw new NullReferenceException($"This player's VIP Level does not exist.{playerCode}");
	}
}
