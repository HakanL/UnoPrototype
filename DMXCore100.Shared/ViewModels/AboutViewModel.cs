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
    public class AboutViewModel : ObservableModelBase
    {
        private readonly ILogger log;

        public AboutViewModel(ILogger<AboutViewModel> logger)
        {
            this.log = logger;

            BackCommand = new DelegateCommand(OnBack);
        }

        public ICommand BackCommand { get; }

        public void OnBack()
        {
            (App.Current as App).RootFrame.Navigate(typeof(Views.HomePage));
        }
    }
}
