using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Loader;

namespace Precursor.Resolvers
{
    public class AssemblyResolver
    {
        [ModuleInitializer]
        public static void Init() 
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.GetCultureInfo("en-US");

            ResolveManagedAssemblies();
            ResolveUnmanagedAssemblies();
        }

        public static void ResolveManagedAssemblies() 
        {
            AssemblyLoadContext.Default.Resolving += static (AssemblyLoadContext ctx, AssemblyName name) =>
            {
                foreach (var file in Directory.EnumerateFiles(Path.Combine(AppContext.BaseDirectory, "Tools"), "*.dll"))
                {
                    AssemblyName an;
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(file);

                        an = new AssemblyName(assembly.GetName().Name);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    if (AssemblyName.ReferenceMatchesDefinition(name, an)) return ctx.LoadFromAssemblyPath(file);
                }

                foreach (var file in Directory.EnumerateFiles(Path.Combine(AppContext.BaseDirectory, @"Tools\Tools"), "*.dll"))
                {
                    AssemblyName an;
                    try
                    {
                        Assembly assembly = Assembly.LoadFile(file);

                        an = new AssemblyName(assembly.GetName().Name);
                    }
                    catch (Exception)
                    {
                        continue;
                    }
                    if (AssemblyName.ReferenceMatchesDefinition(name, an)) return ctx.LoadFromAssemblyPath(file);
                }

                return null;
            };
        }

        public static void ResolveUnmanagedAssemblies()
        {
            AssemblyLoadContext.Default.ResolvingUnmanagedDll += static (Assembly assembly, string dllName) =>
            {
                foreach (var file in Directory.EnumerateFiles(Path.Combine(AppContext.BaseDirectory, "Tools"), "*.dll"))
                {
                    IntPtr handle;
                    try
                    {
                        handle = NativeLibrary.Load(file);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (Path.GetFileName(file) == dllName) return handle;
                }

                foreach (var file in Directory.EnumerateFiles(Path.Combine(AppContext.BaseDirectory, @"Tools\Tools"), "*.dll"))
                {
                    IntPtr handle;
                    try
                    {
                        handle = NativeLibrary.Load(file);
                    }
                    catch (Exception)
                    {
                        continue;
                    }

                    if (Path.GetFileName(file) == dllName) return handle;
                }

                return IntPtr.Zero;
            };
        }
    }
}
