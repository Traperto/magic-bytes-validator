namespace MagicBytesValidator.Formats;

/// <see href="https://de.wikipedia.org/wiki/Scalable_Vector_Graphics"/>
public class Svg : FileByteFilter
{
    public Svg() : base(
        ["image/svg+xml"],
        ["svg", "svgz"]
    )
    {
        /* An svg file needs an opening and a closing svg tag. As some svgs
         * start with an xml opening tag (which seems not to be required though)
         * or comments, we just check whether an svg opening tag exists and if
         * the file ends with a closing tag */
        Anywhere([0x3C, 0x73, 0x76, 0x67]) // "<svg"
            .EndsWithAnyOf([
                [0x3C, 0x2F, 0x73, 0x76, 0x67, 0x3E], // "</svg>"
                [0x3C, 0x2F, 0x73, 0x76, 0x67, 0x3E, 0x0A], // appended LF
                [0x3C, 0x2F, 0x73, 0x76, 0x67, 0x3E, 0x0D, 0x0A], // appended CR LF
            ]);
    }
}