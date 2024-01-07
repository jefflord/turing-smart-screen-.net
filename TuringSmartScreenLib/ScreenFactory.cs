namespace TuringSmartScreenLib;

public static class ScreenFactory
{
    public static IScreen Create(ScreenType type, string name, int? width = null, int? height = null)
    {
        if (type == ScreenType.RevisionB)
        {
            var screen = new TuringSmartScreenRevisionB(name);
            screen.Open();

            if ((screen.Version & 0x10) != 0)
            {
                var screenWrapper = new ScreenWrapperRevisionB1(screen, width ?? 320, height ?? 480)
                {
                    Type = type
                };
                return screenWrapper;
            }
            else
            {
                var screenWrapper = new ScreenWrapperRevisionB0(screen, width ?? 320, height ?? 480)
                {
                    Type = type
                };
                return screenWrapper;
            }
        }

        if (type == ScreenType.RevisionA)
        {
            var screen = new TuringSmartScreenRevisionA(name);
            screen.Open();
            var screenWrapper = new ScreenWrapperRevisionA(screen, width ?? 320, height ?? 480)
            {
                Type = type
            };
            return screenWrapper;
        }

        if (type == ScreenType.RevisionC)
        {
            var screen = new TuringSmartScreenRevisionC(name, true);
            screen.Open();
            var screenWrapper = new ScreenWrapperC(screen, width ?? 800, height ?? 480)
            {
                Type = type
            };
            return screenWrapper;
        }

        throw new NotSupportedException("Unsupported type.");
    }
}
