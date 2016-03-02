﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OctoAwesome.Runtime
{
    /// <summary>
    /// Lädt Erweiterungen nach und stellt deren Typen bereit.
    /// </summary>
    public static class ExtensionManager
    {
        private static List<Assembly> assemblies;

        /// <summary>
        /// Fehler, die beim Laden aufgetreten sind.
        /// </summary>
        public static List<Exception> Errors { get; private set; }

        /// <summary>
        /// Gibt alle Types zurück, die vom angegebenen Type erben.
        /// </summary>
        /// <typeparam name="T">Der zu suchende Type.</typeparam>
        /// <returns></returns>
        public static IEnumerable<Type> GetTypes<T>()
        {
            if (assemblies == null)
            {
                assemblies = new List<Assembly>();
                Errors = new List<Exception>();
                DirectoryInfo dir = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location));

                foreach (var file in dir.GetFiles("*.dll"))
                {
                    try
                    {
                        var assembly = Assembly.LoadFile(file.FullName);
                        assemblies.Add(assembly);
                    }
                    catch (Exception ex)
                    {
                        Errors.Add(ex);
                    }
                }

                DirectoryInfo plugins = new DirectoryInfo(Path.Combine(dir.FullName, "plugins"));
                if (plugins.Exists)
                {
                    foreach (var file in plugins.GetFiles("*.dll"))
                    {
                        try
                        {
                            var assembly = Assembly.LoadFile(file.FullName);
                            assemblies.Add(assembly);
                        }
                        catch (Exception ex)
                        {
                            Errors.Add(ex);
                        }
                    }
                }
            }

            List<Type> result = new List<Type>();
            foreach (var assembly in assemblies)
            {
                foreach (var type in assembly.GetTypes())
                {
                    if (!type.IsPublic) continue;
                    if (type == typeof (T)) continue;

                    if (typeof (T).IsAssignableFrom(type))
                        result.Add(type);
                }
            }
            return result;
        }

        /// <summary>
        /// Gibt Instanzen des angegebenen Typs zurück.
        /// </summary>
        /// <typeparam name="T">Der zu suchende Type.</typeparam>
        /// <returns></returns>
        public static IEnumerable<T> GetInstances<T>()
        {
            List<T> result = new List<T>();
            foreach (var type in GetTypes<T>())
            {
                if (type.IsAbstract) continue;

                result.Add((T)Activator.CreateInstance(type));
            }
            return result;
        }
    }
}