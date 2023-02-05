using AsyncKeyedLock;
using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Application.Logging;
using SharedKernel.Application.Reporting;
using SharedKernel.Application.Security.Cryptography;
using SharedKernel.Application.Serializers;
using SharedKernel.Application.Settings;
using SharedKernel.Application.System;
using SharedKernel.Application.System.Threading;
using SharedKernel.Domain.Security;
using SharedKernel.Infrastructure.Logging;
using SharedKernel.Infrastructure.Reporting;
using SharedKernel.Infrastructure.Security;
using SharedKernel.Infrastructure.Security.Cryptography;
using SharedKernel.Infrastructure.Serializers;
using SharedKernel.Infrastructure.Settings;
using SharedKernel.Infrastructure.System;
using SharedKernel.Infrastructure.System.Threading;

namespace SharedKernel.Infrastructure
{
    /// <summary>
    /// 
    /// </summary>
    public static class AddSharedKernelExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddSharedKernel(this IServiceCollection services)
        {
            return services
                .AddLogging()
                .AddTransient(typeof(ICustomLogger<>), typeof(DefaultCustomLogger<>))
                .AddTransient(typeof(IOptionsService<>), typeof(OptionsService<>))
                .AddTransient<IBase64, Base64>()
                .AddTransient<IBinarySerializer, BinarySerializer>()
                .AddTransient<ICulture, ThreadUiCulture>()
                .AddTransient<ICustomLogger, DefaultCustomLogger>()
                .AddTransient<IDateTime, MachineDateTime>()
                .AddTransient<IEncryptionHexHelper, EncryptionHexHelper>()
                .AddTransient<IGuid, GuidGenerator>()
                .AddTransient<IIdentityService, HttpContextAccessorIdentityService>()
                .AddTransient<IRandomNumberGenerator, RandomNumberGenerator>()
                .AddTransient<IReportRenderer, ReportRenderer>()
                .AddSingleton(new AsyncKeyedLocker<string>(o =>
                {
                    o.PoolSize = 20;
                    o.PoolInitialFill = 1;
                }))
                .AddTransient<ISemaphore, CustomSemaphore>()
                .AddTransient<ISha256, Sha256>()
                .AddTransient<IStreamHelper, StreamHelper>()
                .AddTransient<IStringHelper, StringHelper>()
                .AddTransient<ITripleDes, TripleDes>()
                .AddTransient<IWeb, WebUtils>();
        }
    }
}