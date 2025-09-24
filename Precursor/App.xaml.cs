using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Loader;
using System.Windows;

namespace Precursor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        static App() 
        {
            string searchPath = Path.Combine(AppContext.BaseDirectory, "Tools");

            AssemblyLoadContext.Default.Resolving += (AssemblyLoadContext ctx, AssemblyName name) =>
            {
                try
                {
                    string assemblyPath = Path.Combine(searchPath, $"{name.Name}.dll");
                    if (File.Exists(assemblyPath))
                        return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                }
                catch (Exception)
                {
                    // swallow
                }

                return null;
            };

            AssemblyLoadContext.Default.ResolvingUnmanagedDll += (Assembly assembly, string dllName) =>
            {
                try
                {
                    string dllPath = Path.Combine(searchPath, dllName);
                    if (File.Exists(dllPath))
                        return NativeLibrary.Load(dllPath);
                }
                catch (Exception)
                {
                    // swallow
                }

                return IntPtr.Zero;
            };
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }
    }
}
