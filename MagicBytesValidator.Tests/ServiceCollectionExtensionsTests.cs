namespace MagicBytesValidator.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void Should_register_services_sharing_one_mapping()
    {
        using var provider = new ServiceCollection()
            .AddMagicBytesValidator()
            .BuildServiceProvider();

        var mapping = provider.GetRequiredService<IMapping>();

        Assert.Same(mapping, provider.GetRequiredService<IValidator>().Mapping);
        Assert.Same(mapping, provider.GetRequiredService<IFormFileTypeProvider>().Mapping);
        Assert.Same(mapping, Assert.IsType<StreamFileTypeProvider>(provider.GetRequiredService<IStreamFileTypeProvider>()).Mapping);
        Assert.Same(provider.GetRequiredService<IValidator>(), provider.GetRequiredService<IValidator>());
    }

    [Fact]
    public void Should_configure_mapping_once()
    {
        var calls = 0;
        var customType = new TestFileType().StartsWith([0x01]);

        using var provider = new ServiceCollection()
            .AddMagicBytesValidator(mapping =>
            {
                calls++;
                mapping.Register(customType);
            })
            .BuildServiceProvider();

        Assert.Contains(customType, provider.GetRequiredService<IMapping>().FileTypes);
        Assert.Contains(customType, provider.GetRequiredService<IValidator>().Mapping.FileTypes);
        Assert.Equal(1, calls);
    }
}
