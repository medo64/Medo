using System.IO;
using System.Reflection;

namespace Tests;

internal static class Helper {

    public static byte[] GetResourceBytes(string relativePath) {
        var stream = GetResourceStream(relativePath);
        if (stream == null) { return null; }
        var buffer = new byte[(int)stream.Length];
        stream.Read(buffer, 0, buffer.Length);
        return buffer;
    }

    public static Stream GetResourceStream(string relativePath) {
        if (relativePath == null) { return null; }
        var helperType = typeof(Helper).GetTypeInfo();
        var assembly = helperType.Assembly;
        return assembly.GetManifestResourceStream(helperType.Namespace + "._Resources." + relativePath);
    }

}
