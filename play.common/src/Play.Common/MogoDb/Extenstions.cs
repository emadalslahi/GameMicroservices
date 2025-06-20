using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Play.Common.Entities;
using Play.Common.MongoDB;
using Play.Common.Service.Settings;

namespace Play.Common.Repositories;

public static class Extensions
{


    public static IServiceCollection AddMongoDb(this IServiceCollection services)
    {
        //----------For MongoDB Configuration and Dependency Injection----------------
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String)); // Register the Guid serializer for MongoDB
        BsonSerializer.RegisterSerializer(new DateTimeOffsetSerializer(BsonType.String)); // Register the TimeSpan serializer for MongoDB
                                                                                          //-----------------------------------------------
        services.AddSingleton(serviceProvider =>
     {
        var configuration = serviceProvider.GetRequiredService<IConfiguration>();

         var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
         var serviceSettings = configuration.GetSection(nameof(ServiceSettings)).Get<ServiceSettings>();

         if (serviceSettings == null)
         {
             throw new InvalidOperationException("ServiceSettings configuration is missing.");
         }
         if (mongoDbSettings == null)
         {
             throw new InvalidOperationException("MongoDbSettings configuration is missing.");
         }
         if (string.IsNullOrWhiteSpace(mongoDbSettings.ConnectionString))
         {
             throw new InvalidOperationException("MongoDbSettings.ConnectionString is not configured.");
         }
         var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
         return mongoClient.GetDatabase(serviceSettings.ServiceName);
     });


        return services;
    }

    public static IServiceCollection AddMongoRepository<T>(this IServiceCollection services,
     string collectionName)
        where T : IEntity
    {
        services.AddSingleton<IRepostry<T>>(serviceProvider =>
        {
                    var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            var database = serviceProvider.GetRequiredService<IMongoDatabase>();
            var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings)).Get<MongoDbSettings>();
            
            if (mongoDbSettings == null)
            {
                throw new InvalidOperationException("MongoDbSettings configuration is missing.");
            }
            return new MongoReposotry<T>(database, collectionName);
        });

        return services;
    }
}