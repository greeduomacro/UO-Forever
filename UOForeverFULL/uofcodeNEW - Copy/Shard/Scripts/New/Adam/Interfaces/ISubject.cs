using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Interfaces
{
    public interface ISubject<out T>
        where T : EventArgs
    {
        void Attach(IObserver<T> observer);
        void Detach(IObserver<T> observer);
    }
}
