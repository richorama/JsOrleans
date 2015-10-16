using Orleans;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsOrleans.GrainInterfaces
{
    public interface IJsGrain : IGrainWithStringKey
    {
        Task Become(string javaScript);
        Task<string> Execute(string json);
    }
}
