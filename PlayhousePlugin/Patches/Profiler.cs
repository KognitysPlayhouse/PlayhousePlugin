using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using Exiled.Events;
using Exiled.Events.Extensions;
using HarmonyLib;

namespace PlayhousePlugin
{
    /*
    [HarmonyPatch]
    internal static class EventHandlerPatch
    {
        public static MethodInfo TargetMethod() => typeof(Event).GetMethods().First(x => x.IsGenericMethod).MakeGenericMethod(typeof(EventArgs));
        
        [HarmonyPrefix]
        private static bool Prefix(Events.CustomEventHandler<EventArgs> ev, EventArgs arg)
        {
            if (ev == null)
                return false;

            var watch = new Stopwatch();

            var eventName = ev.GetType().FullName;
            if (!eventName.ToLower().Contains("battery"))
            {

                Log.Debug($"+ {ev.Method.Name}");

                foreach (var handler in ev.GetInvocationList())
                {
                    try
                    {
                        watch.Restart();
                        handler.DynamicInvoke(arg);
                        watch.Stop();
                        Log.Debug(watch.ElapsedMilliseconds +
                                  $"ms {handler.Method.Name}::{handler.Method.ReflectedType?.FullName}");
                    }
                    catch (Exception ex)
                    {
                        Log.Error("Method \"" + handler.Method.Name + "\" of the class \"" +
                                  handler.Method.ReflectedType?.FullName +
                                  "\" caused an exception when handling the event \"" + eventName + "\"");
                        Log.Error((object) ex);
                    }
                }
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(Event), nameof(Event.InvokeSafely), typeof(Events.CustomEventHandler))]
    internal static class SecondEventHandlerPatch
    {
        [HarmonyPrefix]
        private static bool Prefix(Events.CustomEventHandler ev)
        {
            if (ev == null)
                return false;
            
            Log.Debug(ev.Method.Name);

            var watch = new Stopwatch();
            
            var eventName = ev.GetType().FullName;

            foreach (var handler in ev.GetInvocationList())
            {
                try
                {
                    watch.Restart();
                    handler.DynamicInvoke();
                    watch.Stop();
                    Log.Debug($"{watch.ElapsedMilliseconds}ms {handler.Method.Name}::{handler.Method.ReflectedType?.FullName}");
                }
                catch (Exception ex)
                {
                    Log.Error("Method \"" + handler.Method.Name + "\" of the class \"" + handler.Method.ReflectedType?.FullName + "\" caused an exception when handling the event \"" + eventName + "\"");
                    Log.Error((object) ex);
                }
            }
            
            return false;
        }
    }*/
}