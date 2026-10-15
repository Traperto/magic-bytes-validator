namespace MagicBytesValidator.AspNetCore;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers <see cref="IMapping"/>, <see cref="IValidator"/>, <see cref="IStreamFileTypeProvider"/> and
    /// <see cref="IFormFileTypeProvider"/> as singletons sharing the same mapping.
    /// </summary>
    /// <param name="services">Service collection to register the services in</param>
    /// <param name="configureMapping">
    /// Optional. Allows registering custom file types, e.g. <c>mapping => mapping.Register(new CustomType())</c>.
    /// It is called once, when the mapping is created.
    /// </param>
    public static IServiceCollection AddMagicBytesValidator(
        this IServiceCollection services,
        Action<IMapping>? configureMapping = null)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.TryAddSingleton<IMapping>(_ =>
        {
            var mapping = new Mapping();
            configureMapping?.Invoke(mapping);
            return mapping;
        });

        services.TryAddSingleton<IValidator>(provider => new Validator(provider.GetRequiredService<IMapping>()));

        services.TryAddSingleton<IStreamFileTypeProvider>(provider =>
            new StreamFileTypeProvider(provider.GetRequiredService<IMapping>()));

        services.TryAddSingleton<IFormFileTypeProvider>(provider => new FormFileTypeProvider(
            provider.GetRequiredService<IMapping>(),
            provider.GetRequiredService<IValidator>()));

        return services;
    }
}
