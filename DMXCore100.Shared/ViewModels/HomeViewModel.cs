using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using DMXCore.DMXCore100.Contracts;
using Microsoft.Extensions.Logging;
using Prism.Commands;
using Windows.UI.Xaml;

namespace DMXCore.DMXCore100.ViewModels
{
    public class HomeViewModel : ObservableModelBase
    {
        private readonly ILogger log;
        private readonly IIoManager ioManager;
        private bool led1;
        private bool led2;
        private bool led3;
        private bool led4;

        public HomeViewModel(ILogger<HomeViewModel> logger, IIoManager ioManager)
        {
            this.log = logger;
            this.ioManager = ioManager;

            MainCommand = new DelegateCommand(OnMain);
            AboutCommand = new DelegateCommand(OnAbout);

            this.ioManager.PushButtonA.Where(x => x).Subscribe(v => Led1 = !Led1);
            this.ioManager.PushButtonB.Where(x => x).Subscribe(v => Led2 = !Led2);
            this.ioManager.PushButtonC.Where(x => x).Subscribe(v => Led3 = !Led3);
            this.ioManager.PushButtonD.Where(x => x).Subscribe(v => Led4 = !Led4);
        }

        public ICommand AboutCommand { get; }

        public ICommand MainCommand { get; }

        public void OnAbout()
        {
            (App.Current as App).RootFrame.Navigate(typeof(Views.AboutPage));
        }

        public void OnMain()
        {
            (App.Current as App).RootFrame.Navigate(typeof(Views.MainPage));
        }

        public bool Led1
        {
            get => this.led1;
            set
            {
                SetProperty(ref this.led1, value);

                this.ioManager.ControlLed(0, value);
            }
        }

        public bool Led2
        {
            get => this.led2;
            set
            {
                SetProperty(ref this.led2, value);

                this.ioManager.ControlLed(1, value);
            }
        }

        public bool Led3
        {
            get => this.led3;
            set
            {
                SetProperty(ref this.led3, value);

                this.ioManager.ControlLed(2, value);
            }
        }

        public bool Led4
        {
            get => this.led4;
            set
            {
                SetProperty(ref this.led4, value);

                this.ioManager.ControlLed(3, value);
            }
        }
    }
}
