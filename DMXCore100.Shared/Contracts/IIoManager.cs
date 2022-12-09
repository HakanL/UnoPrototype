using System;
using System.Collections.Generic;
using System.Text;

namespace DMXCore.DMXCore100.Contracts
{
    public interface IIoManager : IDisposable
    {
        UnitsNet.Temperature? DoThing();

        IObservable<bool> PushButton { get; }

        IObservable<bool> PushButtonA { get; }

        IObservable<bool> PushButtonB { get; }

        IObservable<bool> PushButtonC { get; }

        IObservable<bool> PushButtonD { get; }

        void UserActivity();

        byte Brightness { get; set; }

        void ControlLed(int index, bool value);
    }
}
