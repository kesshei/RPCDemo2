using System;
using System.Collections.Generic;

namespace RpcInterface
{
    public interface IDemo
    {
        void Say();
        string Say(string msg);
        int Say(string a, int b, List<string> c, Kind kind);
    }
    public enum Kind
    {
        a,
        b
    }
}
