﻿using System.Reflection;
using System.Runtime.Loader;

namespace Permisos.WebApp.Utilidades.Extensiones
{
    public class CustomAssemblyLoadContext: AssemblyLoadContext
    {
        public nint LoadUnmanagedLibrary(string absolutePath)
        {
            return LoadUnmanagedDll(absolutePath);
        }
        protected override nint LoadUnmanagedDll(string unmanagedDllName)
        {
            return LoadUnmanagedDllFromPath(unmanagedDllName);
        }
        protected override Assembly Load(AssemblyName assemblyName)
        {
            throw new NotImplementedException();
        }
    }
}
