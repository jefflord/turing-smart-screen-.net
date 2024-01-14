using System;
using System.CommandLine;
using System.CommandLine.NamingConventionBinder;
using System.Diagnostics;
using SkiaSharp;

using TuringSmartScreenLib;
using TuringSmartScreenLib.Helpers.SkiaSharp;

// ReSharper disable UseObjectOrCollectionInitializer
#pragma warning disable IDE0017

#pragma warning disable CA1812

var rootCommand = new RootCommand("Turing Smart Screen tool");
rootCommand.AddGlobalOption(new Option<string>(new[] { "--revision", "-r" }, static () => "b", "Revision"));
rootCommand.AddGlobalOption(new Option<string>(new[] { "--port", "-p" }, "Port") { IsRequired = true });

static ScreenType GetScreenType(string revision) =>
    String.Equals(revision, "a", StringComparison.OrdinalIgnoreCase)
        ? ScreenType.RevisionA
        : ScreenType.RevisionB;

// Reset
var resetCommand = new Command("reset", "Reset screen");
resetCommand.Handler = CommandHandler.Create((string revision, string port) =>
{
    try
    {
        using var screen = ScreenFactory.Create(GetScreenType(revision), port);
        screen.Reset();
    }
    catch (IOException)
    {
        // Do Nothing
    }
});
rootCommand.Add(resetCommand);

// Clear
var clearCommand = new Command("clear", "Clear screen");
clearCommand.Handler = CommandHandler.Create((string revision, string port) =>
{
    // [MEMO] Type b not supported
    using var screen = ScreenFactory.Create(GetScreenType(revision), port);
    screen.Clear();
});
rootCommand.Add(clearCommand);

// ON
var onCommand = new Command("on", "Screen ON");
onCommand.Handler = CommandHandler.Create((string revision, string port) =>
{
    using var screen = ScreenFactory.Create(GetScreenType(revision), port);
    screen.ScreenOn();
});
rootCommand.Add(onCommand);

// Off
var offCommand = new Command("off", "Screen OFF");
offCommand.Handler = CommandHandler.Create((string revision, string port) =>
{
    using var screen = ScreenFactory.Create(GetScreenType(revision), port);
    screen.ScreenOff();
});
rootCommand.Add(offCommand);

// Brightness
var brightCommand = new Command("bright", "Set brightness");
brightCommand.AddOption(new Option<byte>(new[] { "--level", "-l" }, "Level") { IsRequired = true });
brightCommand.Handler = CommandHandler.Create((string revision, string port, byte level) =>
{
    using var screen = ScreenFactory.Create(GetScreenType(revision), port);
    screen.SetBrightness(level);
});
rootCommand.Add(brightCommand);

// Brightness
var rawCommand = new Command("raw", "Raw command");
rawCommand.AddOption(new Option<string>(new[] { "--commandx", "-c" }, "Command") { IsRequired = true });
rawCommand.Handler = CommandHandler.Create((string revision, string port, string commandx) =>
{
    //Debugger.Launch();
    using var screen = ScreenFactory.Create(GetScreenType(revision), port);
    screen.WriteCommandRaw((byte)int.Parse(commandx));

    //screen.set
    Debugger.Launch();
    for (var i = 0; i < 256; i++)
    {
        if (i == 101 || i == 102 || i == 108 || i == 109 || i == 110 || i == 197)
        {
            continue;
        }



        Console.WriteLine($"Sending {i}");
        screen.WriteCommandRaw((byte)i);
        Thread.Sleep(100);



    }

});
rootCommand.Add(rawCommand);

// Orientation
var orientationCommand = new Command("orientation", "Set orientation");
orientationCommand.AddOption(new Option<string>(new[] { "--mode", "-m" }, "Mode (l or p)"));
orientationCommand.Handler = CommandHandler.Create((string revision, string port, string mode) =>
{
    using var screen = ScreenFactory.Create(GetScreenType(revision), port);
    switch (mode)
    {
        case "l":
        case "landscape":
            screen.Orientation = ScreenOrientation.Landscape;
            break;
        case "p":
        case "portrait":
            screen.Orientation = ScreenOrientation.Portrait;
            break;
    }
});
rootCommand.Add(orientationCommand);

// Image
var imageCommand = new Command("image", "Display image");
imageCommand.AddOption(new Option<string>(new[] { "--file", "-f" }, "Filename") { IsRequired = true });
imageCommand.AddOption(new Option<int>(new[] { "-x" }, static () => 0, "Position x"));
imageCommand.AddOption(new Option<int>(new[] { "-y" }, static () => 0, "Position y"));
imageCommand.Handler = CommandHandler.Create((string revision, string port, string file, int x, int y) =>
{
    using var bitmap = SKBitmap.Decode(File.OpenRead(file));

    using var screen = ScreenFactory.Create(GetScreenType(revision), port);
    var buffer = screen.CreateBuffer(bitmap.Width, bitmap.Height);
    buffer.ReadFrom(bitmap);
    screen.DisplayBuffer(x, y, buffer);
});
rootCommand.Add(imageCommand);

// Fill
var fillCommand = new Command("fill", "Fill screen");
fillCommand.AddOption(new Option<string>(new[] { "--color", "-c" }, static () => "000000", "Color"));
fillCommand.Handler = CommandHandler.Create((string revision, string port, string color) =>
{
    var c = SKColor.Parse(color);
    // TODO fix size & orientation ?
    using var screen = ScreenFactory.Create(GetScreenType(revision), port);

    IScreenBuffer buffer;
    if (screen.Type == ScreenType.RevisionA)
    {
        buffer = screen.CreateBuffer(320, 480);
    }
    else
    {
        buffer = screen.CreateBuffer(480, 320);
    }

    buffer.Clear(c.Red, c.Green, c.Blue);
    screen.DisplayBuffer(buffer);
});
rootCommand.Add(fillCommand);


// rotate
var rotateCommand = new Command("rotate", "Rotate display");
rotateCommand.Handler = CommandHandler.Create((string revision, string port, string text, string align, int x, int y, int size, string font, string color, string background) =>
{

    using var screen = ScreenFactory.Create(GetScreenType(revision), port);


});


// Text
var textCommand = new Command("text", "Display text");
textCommand.AddOption(new Option<string>(new[] { "--text", "-t" }, "Text") { IsRequired = true });
textCommand.AddOption(new Option<int>(new[] { "-x" }, static () => 0, "Position x"));
textCommand.AddOption(new Option<int>(new[] { "-y" }, static () => 0, "Position y"));
textCommand.AddOption(new Option<int>(new[] { "--size", "-s" }, static () => 0, "Size"));
textCommand.AddOption(new Option<string>(new[] { "--font", "-f" }, static () => string.Empty, "Font"));
textCommand.AddOption(new Option<string>(new[] { "--color", "-c" }, static () => "FFFFFF", "Color"));
textCommand.AddOption(new Option<string>(new[] { "--align", "-a" }, static () => "left", "Align"));
textCommand.AddOption(new Option<string>(new[] { "--background", "-b" }, static () => "000000", "Color"));
textCommand.Handler = CommandHandler.Create((string revision, string port, string text, string align, int x, int y, int size, string font, string color, string background) =>
{




    if (false)
    {


        ////y = 100;

        ////screen.Orientation = ScreenOrientation.Portrait;

        //using var bitmap = new SKBitmap((int)Math.Floor(rect.Width), (int)Math.Floor(rect.Height + rect.Width));
        ////using var bitmap = new SKBitmap((int)Math.Floor(rect.Height), (int)Math.Floor(rect.Width));

        //using var canvas = new SKCanvas(bitmap);

        //canvas.RotateDegrees(90, bitmap.Width / 2, bitmap.Height / 2);

        //canvas.Clear(SKColor.Parse(background));
        //canvas.DrawText(text, 0, rect.Height + 40, paint);


        //canvas.Flush();

        //var buffer = screen.CreateBuffer(bitmap.Width, bitmap.Height);
        //buffer.ReadFrom(bitmap);


        //x = screen.Width - (int)rect.Width - 20;
        //y = screen.Height / 2 + 10;

        //if (align == "left")
        //{
        //    screen.DisplayBuffer(x, y, buffer);
        //}
        //else
        //{
        //    screen.DisplayBuffer((screen.Width - (int)Math.Floor(rect.Width) - 10) - x, y, buffer);
        //}
    }
    else if (align == "rigxxxhtxx")
    {
        var sw = Stopwatch.StartNew();
        using var screen = ScreenFactory.Create(GetScreenType(revision), port);

        sw.Restart();

        using var paint = new SKPaint();
        var rect = default(SKRect);
        paint.IsAntialias = true;
        if (size > 0)
        {
            paint.TextSize = size;
        }
        if (!String.IsNullOrEmpty(font))
        {
            paint.Typeface = SKTypeface.FromFamilyName(font);
        }
        paint.Color = SKColor.Parse(color);

        paint.MeasureText(text, ref rect);

        using var bitmap = new SKBitmap((int)Math.Floor(rect.Width) - 10, (int)Math.Floor(rect.Height));

        using var canvas = new SKCanvas(bitmap);
        //canvas.RotateDegrees(90, bitmap.Width / 2, bitmap.Height / 2);
        canvas.Clear(SKColor.Parse(background));
        canvas.DrawText(text, 0, rect.Height, paint);
        canvas.Flush();

        using var buffer = screen.CreateBuffer(bitmap.Width, bitmap.Height);
        buffer.ReadFrom(bitmap);

        if (align == "left")
        {
            screen.DisplayBuffer(x, y, buffer);
        }
        else
        {
            screen.DisplayBuffer((screen.Width - (int)Math.Floor(rect.Width) - 10) - x, y, buffer);
        }
    }
    else
    {
        for (var xxx = 0; xxx < 23423; xxx++)
        {

            try
            {
                Thread.Sleep(100);
                x = 310;
                y = 0;
                align = "right";

                Do90Test(revision, port, text, align, x, y, size, font, color, background);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                Thread.Sleep(500);
            }

        }


    }


    static void Do90Test(string revision, string port, string text, string align, int x, int y, int size, string font, string color, string background)
    {


        text = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var rand = new Random();
        var nextSize = rand.Next(3, 9);


        text = $"{nextSize}:{text.Substring(0, nextSize)}";


        using (var screen = ScreenFactory.Create(GetScreenType(revision), port))
        {


            using var paint = new SKPaint();
            var rect = default(SKRect);
            paint.IsAntialias = true;
            if (size > 0)
            {
                paint.TextSize = size;
            }
            if (!String.IsNullOrEmpty(font))
            {
                paint.Typeface = SKTypeface.FromFamilyName(font);
            }

            paint.Color = SKColor.Parse(color);
            paint.MeasureText(text, ref rect);



            IScreenBuffer buffer;
            {
                var c = SKColor.Parse("FFFF4F");
                // TODO fix size & orientation ?


                if (screen.Type == ScreenType.RevisionA)
                {
                    buffer = screen.CreateBuffer(320, 480);
                }
                else
                {
                    buffer = screen.CreateBuffer(480, 320);
                }

                buffer.Clear(c.Red, c.Green, c.Blue);
                screen.DisplayBuffer(buffer);

            }

            //var bitmap = new SKBitmap((int)Math.Floor(rect.Width), (int)Math.Floor(rect.Height));
            var bitmap = new SKBitmap((int)Math.Floor(rect.Height), (int)Math.Floor(rect.Width));
            var canvas = new SKCanvas(bitmap);

            //canvas.RotateDegrees(10, bitmap.Width / 2, bitmap.Height / 2);
            canvas.RotateDegrees(90, 0, 0);

            canvas.Clear(SKColor.Parse(background));
            //canvas.DrawText(text, 0, rect.Height, paint);


            //canvas.DrawText(text, 0, rect.Height, paint);

            canvas.DrawText(text, 0, 0, paint);

            //canvas.RotateDegrees(90, bitmap.Width / 2, bitmap.Height / 2);


            canvas.Flush();

            buffer = screen.CreateBuffer(bitmap.Width, bitmap.Height);
            buffer.ReadFrom(bitmap);


            var realX = 320 - bitmap.Width - y;
            var realY = 0 + x;

            if (align == "right")
            {
                realY = 480 - bitmap.Height - x;
            }

            if (realX < 0)
            {
                realX = 0;
            }
            //var xx = realX - rect.Width;


            if (realY + bitmap.Height > 480)
            {
                realY = 480 - bitmap.Height;
            }
            if (realY - bitmap.Width < 0)
            {
                realY = 0;
            }

            screen.DisplayBuffer(realX, realY, buffer);

            screen.Clear();
            buffer.Dispose();
        }

    }
});
rootCommand.Add(textCommand);

return rootCommand.Invoke(args);
