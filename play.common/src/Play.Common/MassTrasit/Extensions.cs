using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Play.Common.Service.Settings;

namespace Play.Common.MassTrasit
{
    public static class Extensions
    {
        public static IServiceCollection AddMassTransitWithRabbitMQ(this IServiceCollection services)
        {

            services.AddMassTransit(config =>
            {
                config.AddConsumers(Assembly.GetEntryAssembly()); // Automatically register all consumers in the assembly

                config.UsingRabbitMq((context, cfg) =>
                {

                    var configuration = context.GetRequiredService<IConfiguration>();
                    var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();
                    if (serviceSettings == null)
                    {
                        throw new InvalidOperationException("ServiceSettings configuration is missing.");
                    }

                    var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings)).Get<RabbitMQSettings>();
                    if (rabbitMQSettings == null)
                    {
                        throw new InvalidOperationException("RabbitMQSettings configuration is missing.");
                    }
                    else
                    {

                        cfg.Host(rabbitMQSettings.Host, h =>
                        {
                            h.Username(rabbitMQSettings.UserName); // Use default username
                            h.Password(rabbitMQSettings.Password); // Use default password
                        });
                        cfg.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                        cfg.UseMessageRetry(fktry => { fktry.Interval(3, TimeSpan.FromSeconds(5)); });
                        //cfg.Host(rabbitMQSettings.Host, rabbitMQSettings.Port, rabbitMQSettings.VirtualHost, h =>
                        //{
                        //    h.Username(rabbitMQSettings.UserName);
                        //    h.Password(rabbitMQSettings.Password);
                        //});
                    }
                    //cfg.UseHealthCheck(context);
                    //cfg.ConfigureEndpoints(context);
                });
            });
            return services;
        }

    }
}