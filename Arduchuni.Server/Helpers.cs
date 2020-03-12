using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Threading;
using Harmony;

namespace Arduchuni.Server {
    public static class Helpers {
        
        private static HarmonyInstance _harmony = HarmonyInstance.Create("co.logu.arduchuni.server.helpers");

        public static void PatchNotifyPropertyChanged<T>() where T : PropertyChangedInvoker {
            var type = typeof(T);
            var patch = typeof(Helpers).GetMethod("PropertyChangedPostfix", BindingFlags.NonPublic | BindingFlags.Static);
            Debug.Assert(patch != null, nameof(patch) + " != null");
            patch = patch.MakeGenericMethod(type);

            foreach (var method in type.GetProperties().Select(p => p.SetMethod)) {
                _harmony.Patch(method, new HarmonyMethod(patch));
            }
        }

        // ReSharper disable InconsistentNaming
        // ReSharper disable once UnusedMember.Local
        private static void PropertyChangedPostfix<T>(MethodBase __originalMethod, T __instance) where T : PropertyChangedInvoker {
            __instance.InvokePropertyChanged(__originalMethod.Name.Substring(4)); // remove set_
        }
        // ReSharper restore InconsistentNaming

        public abstract class PropertyChangedInvoker : INotifyPropertyChanged {
            public event PropertyChangedEventHandler PropertyChanged;
            public virtual void InvokePropertyChanged(string propertyName) {
                Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() => {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
                }));
            }
        }
    }
}
