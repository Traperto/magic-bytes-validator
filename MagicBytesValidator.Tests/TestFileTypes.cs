namespace MagicBytesValidator.Tests;

public class TestFileType : FileByteFilter
{
    public TestFileType(string[] mimeTypes, string[] extensions) : base(mimeTypes, extensions)
    {
    }

    public TestFileType() : this([Guid.NewGuid().ToString()], [Guid.NewGuid().ToString()])
    {

    }
}

class ParentFileType : FileByteFilter
{

    public ParentFileType() : this(["parent"], ["parent"])
    {
    }

    public ParentFileType(string[] mimeTypes, string[] extensions) : base(mimeTypes, extensions)
    {
        StartsWith([1, 2, 3]);
    }
}

class ChildFileType : ParentFileType
{

    public ChildFileType() : this(["child"], ["child"])
    {
    }

    public ChildFileType(string[] mimeTypes, string[] extensions) : base(mimeTypes, extensions)
    {
        EndsWith([4, 5, 6]);
    }
}

class GrandchildFileType : ChildFileType
{
    public GrandchildFileType() : base(["grandchild"], ["grandchild"])
    {
        Anywhere([10, 11, 12]);
    }
}