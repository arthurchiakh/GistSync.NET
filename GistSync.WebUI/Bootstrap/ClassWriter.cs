using System.Collections.Generic;

namespace GistSync.WebUI.Bootstrap;

public static class ClassWriter
{
    private readonly static Dictionary<Size, string> _sizeCodes = new()
    {
        { Size.Small , "sm"},
        { Size.Default , string.Empty},
        { Size.Large , "lg"},
        { Size.ExtraLarge , "xl"},
    };

    public static string Write(string className, Size size, string defaultClass = "")
    {
        return size == Size.Default ? defaultClass: $"{className}-{_sizeCodes[size]}";
    }
}