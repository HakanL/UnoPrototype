using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Serilog;
using Uno.UI.Runtime.Skia;
using Windows.UI.Core;

namespace DMXCore.DMXCore100
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine($"Starting up at {DateTime.Now:g}");

            try
            {
                Console.CursorVisible = false;

                FrameBufferHost theHost = null;

                var host = new FrameBufferHost(() =>
                {
                    // Framebuffer applications don't have a WindowManager to rely
                    // on. To close the application, we can hook onto CoreWindow events
                    // which dispatch keyboard input, and close the application as a result.
                    // This block can be moved to App.xaml.cs if it does not interfere with other
                    // platforms that may use the same keys.
                    CoreWindow.GetForCurrentThread().KeyDown += (s, e) =>
                    {
                        if (e.VirtualKey == Windows.System.VirtualKey.F12)
                        {
#pragma warning disable Uno0001 // Uno type or member is not implemented
                            Application.Current.Exit();
#pragma warning restore Uno0001 // Uno type or member is not implemented
                        }
                    };

                    CoreWindow.GetForCurrentThread().PointerMoved += (s, e) =>
                    {
                        (Application.Current as App).IoManager?.UserActivity();
                    };

                    CoreWindow.GetForCurrentThread().PointerPressed += (s, e) =>
                    {
                        (Application.Current as App).IoManager?.UserActivity();

                        var renderer = typeof(FrameBufferHost).GetField("_renderer", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(theHost);
                        Console.WriteLine($"renderer: {renderer}");

                        var fbDev = renderer.GetType().GetProperty("FrameBufferDevice").GetValue(renderer);
                        Console.WriteLine($"fbDev: {fbDev}");

                        var screenInfo = fbDev.GetType().GetField("_screenInfo", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(fbDev);
                        Console.WriteLine($"screenInfo: {screenInfo}");

                        DumpObject(screenInfo);
                    };

                    return new App();
                });

                theHost = host;
                host.Run();
            }
            finally
            {
                Log.CloseAndFlush();
                Console.CursorVisible = true;
            }
        }

        static void DumpObject(object o, string prefix = null)
        {
            var fields = o.GetType().GetFields();
            foreach (var field in fields)
            {
                object v = field.GetValue(o);

                if (prefix == null)
                    Console.WriteLine($"{field.Name} = {v}");
                else
                    Console.WriteLine($"{prefix}.{field.Name} = {v}");

                if (v is not null)
                {
                    var vType = v.GetType();
                    if (vType.IsClass || !vType.IsPrimitive)
                        DumpObject(v, $"{prefix}.{field.Name}");
                }
            }
        }
    }
}
