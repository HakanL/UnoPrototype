using DMXCore.DMXCore100.Contracts;
using DMXCore.DMXCore100.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace DMXCore.DMXCore100.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public static readonly Guid AcnSourceId = new("{321E22B6-9DE7-475B-BCBB-73DB69AEB53D}");
        public static readonly string AcnSourceName = "DMX Core 100";

        private readonly ILogger log;
        private int counter;
        private readonly IIoManager ioManager;
        private readonly DispatcherTimer timer;

        public MainPage()
        {
            this.InitializeComponent();

            // Get a local instance of the container
            var container = ((App)App.Current).Container;

            Console.WriteLine("Step 1");

            this.log = container.GetRequiredService<ILogger<MainPage>>();

            Console.WriteLine("Step 2");
            this.log.LogInformation("Hello logger");
            Console.WriteLine("Step 3");


            // Request an instance of the ViewModel and set it to the DataContext
            DataContext = ActivatorUtilities.GetServiceOrCreateInstance(container, typeof(MainViewModel));

            Console.WriteLine("Step 4");

            this.ioManager = container.GetRequiredService<IIoManager>();
            
            Console.WriteLine("Step 5");

            Console.WriteLine("Step 6");

            this.timer = new DispatcherTimer();
            this.timer.Tick += Timer_Tick;
            this.timer.Interval = TimeSpan.FromSeconds(1);

            this.ioManager.PushButton.Subscribe(async v =>
            {
                await Window.Current.Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    labelHelloWorld.Text = $"Push button {++this.counter}";
                });
            });

            this.ioManager.PushButtonA.Subscribe(async v =>
            {
                await Window.Current.Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    labelHelloWorld.Text = $"Push button A {++this.counter}";
                });
            });

            this.ioManager.PushButtonB.Subscribe(async v =>
            {
                await Window.Current.Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    labelHelloWorld.Text = $"Push button B {++this.counter}";
                });
            });

            this.ioManager.PushButtonC.Subscribe(async v =>
            {
                await Window.Current.Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    labelHelloWorld.Text = $"Push button C {++this.counter}";
                });
            });

            this.ioManager.PushButtonD.Subscribe(async v =>
            {
                await Window.Current.Dispatcher.RunAsync(global::Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
                {
                    labelHelloWorld.Text = $"Push button D {++this.counter}";
                });
            });
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            this.timer.Start();
        }

        private void Timer_Tick(object sender, object e)
        {
            var reading = this.ioManager.DoThing();

            if (reading.HasValue)
                labelTemp.Text = $"Temp: {reading.Value.DegreesCelsius:N2}°C";
            else
                labelTemp.Text = "N/A";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            labelHelloWorld.Text = $"Jahapp {++this.counter}";
        }
    }
}
