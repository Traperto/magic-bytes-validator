namespace MagicBytesValidator.Tests.Http;

public class AddMagicBytesValidator
{
    [Fact]
    public void Should_register_all_services_with_shared_mapping()
    {
        using var provider = new ServiceCollection()
            .AddMagicBytesValidator()
            .BuildServiceProvider();

        var mapping = provider.GetRequiredService<IMapping>();

        Assert.Same(mapping, provider.GetRequiredService<IValidator>().Mapping);
        Assert.Same(mapping, provider.GetRequiredService<IStreamFileTypeProvider>().Mapping);
        Assert.Same(mapping, provider.GetRequiredService<IFormFileTypeProvider>().Mapping);
    }

    [Fact]
    public void Should_register_services_as_singletons()
    {
        using var provider = new ServiceCollection()
            .AddMagicBytesValidator()
            .BuildServiceProvider();

        Assert.Same(provider.GetRequiredService<IValidator>(), provider.GetRequiredService<IValidator>());
        Assert.Same(
            provider.GetRequiredService<IFormFileTypeProvider>(),
            provider.GetRequiredService<IFormFileTypeProvider>());
    }

    [Fact]
    public void Should_configure_mapping_once()
    {
        var customType = new TestFileType(["traperto/trp"], ["trp"]).StartsWith([0x74, 0x72, 0x70]);
        var configureCalls = 0;

        using var provider = new ServiceCollection()
            .AddMagicBytesValidator(mapping =>
            {
                configureCalls++;
                mapping.Register(customType);
            })
            .BuildServiceProvider();

        Assert.Same(customType, provider.GetRequiredService<IMapping>().FindByExtension("trp"));
        Assert.Same(customType, provider.GetRequiredService<IValidator>().Mapping.FindByMimeType("traperto/trp"));
        Assert.Equal(1, configureCalls);
    }

    [Fact]
    public void Should_keep_existing_registrations()
    {
        var mapping = new Mapping();

        using var provider = new ServiceCollection()
            .AddSingleton<IMapping>(mapping)
            .AddMagicBytesValidator()
            .BuildServiceProvider();

        Assert.Same(mapping, provider.GetRequiredService<IFormFileTypeProvider>().Mapping);
    }
}
