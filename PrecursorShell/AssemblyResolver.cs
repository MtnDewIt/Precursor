using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace PrecursorShell
{
    public static class AssemblyResolver
    {
        static readonly string[] searchPaths =
        [
            Path.Combine(AppContext.BaseDirectory, "Tools"),
            Path.Combine(AppContext.BaseDirectory, @"Tools\Tools")
        ];

        [ModuleInitializer]
        public static void Init() 
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");

            ConfigureAssemblyResolution();
        }

        public static void ConfigureAssemblyResolution()
        {
            AssemblyLoadContext.Default.Resolving += (ctx, name) =>
            {
                foreach (string path in searchPaths)
                {
                    try
                    {
                        string assemblyPath = Path.Combine(path, $"{name.Name}.dll");
                        if (File.Exists(assemblyPath))
                            return AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
                    }
                    catch (Exception)
                    {
                        // swallow
                    }
                }

                return null;
            };

            AssemblyLoadContext.Default.ResolvingUnmanagedDll += (assembly, dllName) =>
            {
                foreach (string path in searchPaths)
                {
                    try
                    {
                        string dllPath = Path.Combine(path, dllName);
                        if (File.Exists(dllPath))
                            return NativeLibrary.Load(dllPath);
                    }
                    catch (Exception)
                    {
                        // swallow
                    }
                }

                return nint.Zero;
            };
        }
    }
}
