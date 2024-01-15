using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Tgs.VipLevelService;

namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
	public static IServiceCollection AddVipLevelServices(
		this IServiceCollection services,
		Action<IServiceProvider, DbContextOptionsBuilder> dbContextOptions,
		Action<RedisOptions, IServiceProvider> redisOptions)
		=> services
		.AddOptions<RedisOptions>()
		.Configure(redisOptions)
		.Services
		.AddSingleton<RedisConnectionManager>()
		.AddVipLevelCore()
		.AddVipLevelRepositories(
			dbContextOptions: dbContextOptions,
			redisDatabaseResolver: (sp) => sp.GetRequiredService<RedisConnectionManager>().Database);
}
