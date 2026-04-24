namespace MagicBytesValidator.Tests.Models;

public class AudioFormatMatches
{
    [Fact]
    public void Should_match_wav()
    {
        var wav = new Wav();

        // RIFF....WAVE header
        var wavTestData = new byte[]
        {
            0x52, 0x49, 0x46, 0x46, // RIFF
            0x24, 0x08, 0x00, 0x00, // file size (arbitrary)
            0x57, 0x41, 0x56, 0x45  // WAVE
        };

        Assert.True(wav.Matches(wavTestData));
    }

    [Fact]
    public void Should_not_match_wav_with_avi_riff()
    {
        var wav = new Wav();

        // RIFF....AVI  (not WAVE)
        var aviTestData = new byte[]
        {
            0x52, 0x49, 0x46, 0x46, // RIFF
            0x24, 0x08, 0x00, 0x00, // file size
            0x41, 0x56, 0x49, 0x20  // AVI
        };

        Assert.False(wav.Matches(aviTestData));
    }

    [Fact]
    public void Should_not_match_wav_with_invalid_data()
    {
        var wav = new Wav();

        var invalidTestData = new byte[] { 0x00, 0x00, 0x00, 0x00 };

        Assert.False(wav.Matches(invalidTestData));
    }

    [Fact]
    public void Should_match_m4a_with_m4a_brand()
    {
        var m4a = new M4a();

        // ftyp box with M4A brand
        var m4aTestData = new byte[]
        {
            0x00, 0x00, 0x00, 0x20, // box size
            0x66, 0x74, 0x79, 0x70, // ftyp
            0x4D, 0x34, 0x41, 0x20  // M4A
        };

        Assert.True(m4a.Matches(m4aTestData));
    }

    [Fact]
    public void Should_match_m4a_with_m4b_brand()
    {
        var m4a = new M4a();

        // ftyp box with M4B brand (audiobook variant)
        var m4bTestData = new byte[]
        {
            0x00, 0x00, 0x00, 0x20, // box size
            0x66, 0x74, 0x79, 0x70, // ftyp
            0x4D, 0x34, 0x42, 0x20  // M4B
        };

        Assert.True(m4a.Matches(m4bTestData));
    }

    [Fact]
    public void Should_not_match_m4a_with_isom_brand()
    {
        var m4a = new M4a();

        // ftyp box with isom brand (generic MP4 video)
        var mp4TestData = new byte[]
        {
            0x00, 0x00, 0x00, 0x20, // box size
            0x66, 0x74, 0x79, 0x70, // ftyp
            0x69, 0x73, 0x6F, 0x6D  // isom
        };

        Assert.False(m4a.Matches(mp4TestData));
    }

    [Fact]
    public void Should_match_flac()
    {
        var flac = new Flac();

        // fLaC magic marker followed by metadata block header
        var flacTestData = new byte[]
        {
            0x66, 0x4C, 0x61, 0x43, // fLaC
            0x00, 0x00, 0x00, 0x22  // STREAMINFO block header
        };

        Assert.True(flac.Matches(flacTestData));
    }

    [Fact]
    public void Should_not_match_flac_with_invalid_data()
    {
        var flac = new Flac();

        var invalidTestData = new byte[] { 0x66, 0x4C, 0x00, 0x00 };

        Assert.False(flac.Matches(invalidTestData));
    }
}
