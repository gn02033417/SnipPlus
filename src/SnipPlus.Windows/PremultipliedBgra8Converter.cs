namespace SnipPlus.Windows;

public static class PremultipliedBgra8Converter
{
    public static byte[] FromStraightBgra(ReadOnlySpan<byte> straightBgra)
    {
        if (straightBgra.Length % 4 != 0)
        {
            throw new ArgumentException("BGRA8 data must contain complete pixels.", nameof(straightBgra));
        }

        var result = straightBgra.ToArray();
        for (var index = 0; index < result.Length; index += 4)
        {
            var alpha = result[index + 3];
            result[index] = Premultiply(result[index], alpha);
            result[index + 1] = Premultiply(result[index + 1], alpha);
            result[index + 2] = Premultiply(result[index + 2], alpha);
        }

        return result;
    }

    private static byte Premultiply(byte channel, byte alpha)
    {
        return (byte)((channel * alpha + 127) / 255);
    }
}
