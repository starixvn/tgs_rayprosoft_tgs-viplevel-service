using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Json.Patch;

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

	public IAsyncEnumerable<VipLevelInfo> GetBrandAllVipLevelsAsync(Guid brandId, CancellationToken cancellationToken = default)
		=> _httpClient
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

		var response = await _httpClient.PutAsync(
			$"/vip/{brandId}",
			requestContent,
			cancellationToken).ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();

		return await response.Content.ReadFromJsonAsync<Guid>(_SerializerOptions, cancellationToken);
	}

	public async ValueTask UpdateVipLevelAsync(
		Guid vipId,
		VipLevelUpdateInfo updateInfo,
		string modifier,
		CancellationToken cancellationToken = default)
	{
		var patch = VipLevelUpdateInfo.Default.CreatePatch(updateInfo);

		var requestContent = JsonContent.Create(patch, options: _SerializerOptions);
		requestContent.Headers.Add("Modifier", modifier);

		var response = await _httpClient
			.PatchAsync($"/vip/{vipId}", requestContent, cancellationToken)
			.ConfigureAwait(false);

		_ = response.EnsureSuccessStatusCode();
	}
}
