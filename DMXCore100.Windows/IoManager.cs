using DMXCore.DMXCore100.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace DMXCore.DMXCore100
{
    public class IoManager : IIoManager
    {
        public IoManager()
        {
        }

        public IObservable<bool> PushButton => Observable.Never<bool>();

        public IObservable<bool> PushButtonA => Observable.Never<bool>();

        public IObservable<bool> PushButtonB => Observable.Never<bool>();

        public IObservable<bool> PushButtonC => Observable.Never<bool>();

        public IObservable<bool> PushButtonD => Observable.Never<bool>();

        public byte Brightness { get => 0; set { } }

        public void ControlLed(int index, bool value)
        {
            // Do nothing
        }

        public void Dispose()
        {
        }

        public UnitsNet.Temperature? DoThing()
        {
            return null;
        }

        public void UserActivity()
        {
            // Do nothing
        }
    }
}
