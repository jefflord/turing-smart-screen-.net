namespace TuringSmartScreenLib;

using System;

#pragma warning disable CA1819
#pragma warning disable IDE0032
public sealed class TuringSmartScreenBufferC : IScreenBuffer
{
    internal byte[] ImgBuffer { get; set; } = Array.Empty<byte>();

    public int Width { get; private set; }

    public int Height { get; private set; }

    public int Length => ImgBuffer.Length;

    public void SetPixel(int x, int y, byte r, byte g, byte b)
    {
        ImgBuffer[(y * Width) + x] = r;
        ImgBuffer[(y * Width) + x + 1] = g;
        ImgBuffer[(y * Width) + x + 2] = b;
    }

    public void Clear(byte r = 0, byte g = 0, byte b = 0) => ImgBuffer = Array.Empty<byte>();

    public void SetRGB(int sw, int sh, byte[] buffer)
    {
        Width = sw;
        Height = sh;
        ImgBuffer = buffer;
    }

    internal bool IsEmpty()
    {
        return ImgBuffer.Length == 0;
    }
}
#pragma warning restore IDE0032
#pragma warning restore CA1819
