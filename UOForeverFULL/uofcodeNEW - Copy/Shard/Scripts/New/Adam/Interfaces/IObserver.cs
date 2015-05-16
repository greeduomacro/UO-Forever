using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Server.Interfaces
{
    public interface IObserver<in T>
        where T : EventArgs
    {
        void Update(Object sender, T e);
    }
}
