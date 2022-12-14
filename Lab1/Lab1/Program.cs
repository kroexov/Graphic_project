using Avalonia;
using Avalonia.ReactiveUI;
using System;
using Lab1.Models;

namespace Lab1
{
    class Program
    {
        // // Initialization code. Don't use any Avalonia, third-party APIs or any
        // // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // // yet and stuff might break.
        // [STAThread]
        // public static void Main(string[] args) => BuildAvaloniaApp()
        //     .StartWithClassicDesktopLifetime(args);
        //
        // // Avalonia configuration, don't remove; also used by visual designer.
        // public static AppBuilder BuildAvaloniaApp()
        //     => AppBuilder.Configure<App>()
        //         .UsePlatformDetect()
        //         .LogToTrace()
        //         .UseReactiveUI();
        public static void Main()
        {
            PnmServices pnm = new PnmServices();
            pnm.ReadFile("C:\\Users\\dewor\\Desktop\\ffffff.ppm", new []{true, true, true});
            pnm.FilterImage(TypeFilter.ThresholdFilteringByOcu);
        }
    }
}