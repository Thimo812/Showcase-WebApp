using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;

namespace Showcase_WebApp.Models
{
    public class Connections<T> where T : Hub
    {
        public ConcurrentDictionary<string, HubCallerContext> All { get; } = new();
    }
}
