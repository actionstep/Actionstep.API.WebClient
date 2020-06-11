using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace Actionstep.API.WebClient
{
    public static class Utilities
    {
        public static ValueTask<object> SaveAs(this IJSRuntime js, string filename, byte[] data)
           => js.InvokeAsync<object>("saveAsFile", filename, Convert.ToBase64String(data));
    }
}