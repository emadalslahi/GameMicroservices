namespace Play.Common.Service.Settings;

using System;
public class MongoDbSettings
{
    public string Host { get; init; } = "localhost";
    public string Port { get; init; } = "27017";
    public string ConnectionString => $"mongodb://{Host}:{Port}";
    public string ItemsCollectionName { get; init; } = "items";
    public TimeSpan? RequestTimeout { get; init; } = TimeSpan.FromSeconds(30);
}