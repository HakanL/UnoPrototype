using DMXCore.DMXCore100.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Text;

namespace DMXCore.DMXCore100.ViewModels
{
    public class MainViewModel : ObservableModelBase
    {
        private ILogger log;
        protected readonly IIoManager ioManager;
        private string message = "Idle";
        private TimeSpan playerClock;

        public MainViewModel(ILogger<MainViewModel> logger, IIoManager ioManager)
        {
            this.log = logger;
            this.ioManager = ioManager;
        }

        public TimeSpan PlayerClock
        {
            get => this.playerClock;
            set => SetProperty(ref this.playerClock, value);
        }

        public string Message { get => this.message; set => SetProperty(ref this.message, value); }

        public IList<string> TestList => new List<string>()
        {
            "safasdfsda",
            "safasdfsda1",
            "safasdfsda2",
            "safasdfsda3",
            "safasdfsda4",
            "safasdfsda5",
            "safasdfsda6",
            "safasdfsda7",
            "safasdfsda8",
            "safasdfsda9",
            "safasdfsda10",
            "safasdfsda11",
            "safasdfsda12",
            "safasdfsda13",
            "safasdfsda14",
            "safasdfsda15",
            "safasdfsda16"
        };

        //public ICommand TriggerPattern1Command => new Command<object>(ParamArrayAttribute =>
        //{

        //});
    }
}
