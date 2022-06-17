using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RPCServerLib.Server
{
    public interface IRpcClient
    {
        void Start();
        string Send(string message);
        T Resolve<T>() where T : class;
        void Close();
    }
}
