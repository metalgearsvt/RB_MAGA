using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using ff14bot.AClasses;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Behavior;
using TreeSharp;
using Action = TreeSharp.Action;

namespace MetalBuddyLoader
{
    public class MetalBuddyLoader : BotPlugin
    {
        public MetalBuddyLoader()
        {
            if (started) return;
            started = true;
            //LoadPlugin();
        }
        #region Meta Data

        private static bool started = false;
        private const string PluginClass = "MetalBuddy.Plugin";
        private static string ProjectName = "Metal's Active Gameplay Assistant";
        private static readonly string PluginAssembly = Path.Combine(Environment.CurrentDirectory, @"Plugins\MetalBuddy\MetalBuddy.dll");
		private static readonly string greyMagicAssembly = Path.Combine(Environment.CurrentDirectory, @"GreyMagic.dll");
		private static readonly string DXAsm = Path.Combine(Environment.CurrentDirectory, @"SlimDX.dll");
        private static readonly object ObjLock = new object();

        #endregion

        #region Overrides
        public override bool WantButton => true;
        public override string Name => ProjectName;
        private static MethodInfo StartFunc { get; set; }
		private static MethodInfo StopFunc { get; set; }
        private static MethodInfo ButtonFunc { get; set; }
		private static MethodInfo ShutdownFunc { get; set; }
        private static MethodInfo RootFunc { get; set; }
        private static MethodInfo InitFunc { get; set; }
        private static MethodInfo PulseFunc { get; set; }

		public override Version Version {
			get {
				return new Version(0, 0, 1);
			}
		}
		
		public override string Author {
			get {
				return "MetalSVT";
			}
		}
		
        public override void OnButtonPress()
        {
            if (Plugin == null) { LoadPlugin(); }
            if (Plugin != null) { ButtonFunc.Invoke(Plugin, null); }
        }
		
		public override void OnEnabled() {
			if (Plugin == null) {
				LoadPlugin();
			}
			if(Plugin != null) {
				StartFunc.Invoke(Plugin, null);
			}
		}
		
		public override void OnDisabled() {
			if (Plugin == null) {
				LoadPlugin();
			}
			if(Plugin != null) {
				StopFunc.Invoke(Plugin, null);
			}
		}
		
        public override void OnInitialize()
        {
            if (Plugin == null)
            {
                LoadPlugin();
            }
            if (Plugin != null)
            {
                InitFunc.Invoke(Plugin, null);
            }
        }
		
		public override void OnShutdown()
        {
            if (Plugin == null)
            {
                LoadPlugin();
            }
            if (Plugin != null)
            {
                ShutdownFunc.Invoke(Plugin, null);
            }
        }

        public override void OnPulse()
        {
            if (Plugin == null)
            {
                LoadPlugin();
            }
            if (Plugin != null)
            {
                PulseFunc.Invoke(Plugin, null);
            }
        }

        #endregion

        #region Injections

        private static object Plugin { get; set; }

        #endregion

        #region Inject Methods

        private static object Load()
        {
            RedirectAssembly();

            var assembly = LoadAssembly(PluginAssembly);
            if (assembly == null)
            {
                return null;
            }

            Type baseType;
            try
            {
                baseType = assembly.GetType(PluginClass);
            }
            catch (Exception e)
            {
                Log(e.ToString());
                return null;
            }

            object plugin;
            try
            {
                plugin = Activator.CreateInstance(baseType);
            }
            catch (Exception e)
            {
                Log(e.ToString());
                return null;
            }

            Log(plugin != null
                ? "Loaded successfully."
                : "Could not load. This can be due to a new version of Rebornbuddy being released. An update should be ready soon.");

            return plugin;
        }

        private static Assembly LoadAssembly(string path)
        {

            if (!File.Exists(path)) { return null; }
            
            Assembly assembly = null;
            try
            {
                assembly = Assembly.LoadFrom(path);
            }
            catch (Exception e)
            {
                Logging.WriteException(e);
                var typeLoadException = e as ReflectionTypeLoadException;
                if (typeLoadException != null)
                {
                    var loaderExceptions = typeLoadException.LoaderExceptions;
                    foreach(var ee in loaderExceptions)
                        Logging.WriteException(ee);
                }
            }

            return assembly;
        }

        private static void LoadPlugin()
        {
            lock (ObjLock)
            {
                if (Plugin != null)
                {
                    return;
                }
                Plugin = Load();

                if (Plugin == null)
                {
                    return;
                }

                StartFunc = Plugin.GetType().GetMethod("OnEnabled");
                StopFunc = Plugin.GetType().GetMethod("OnDisabled");
				ShutdownFunc = Plugin.GetType().GetMethod("OnShutdown");
                ButtonFunc = Plugin.GetType().GetMethod("OnButtonPress");
                InitFunc = Plugin.GetType().GetMethod("OnInitialize");
				PulseFunc = Plugin.GetType().GetMethod("OnPulse");
                if (InitFunc != null)
                    InitFunc.Invoke(Plugin, null);

            }
        }

        #endregion

        #region Helper Methods

        private static void Log(string message)
        {
            Logging.Write(Colors.Magenta, $"[MAGALoader] {message}");
        }

        public static void RedirectAssembly()
        {
            ResolveEventHandler handler = (sender, args) =>
            {
                string name = Assembly.GetEntryAssembly().GetName().Name;
                var requestedAssembly = new AssemblyName(args.Name);
                return requestedAssembly.Name != name ? null : Assembly.GetEntryAssembly();
            };

            AppDomain.CurrentDomain.AssemblyResolve += handler;

            ResolveEventHandler greyMagicHandler = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);
                return requestedAssembly.Name != "GreyMagic" ? null : Assembly.LoadFrom(greyMagicAssembly);
            };

            AppDomain.CurrentDomain.AssemblyResolve += greyMagicHandler;
			
			ResolveEventHandler DXHandlerAsm = (sender, args) =>
            {
                var requestedAssembly = new AssemblyName(args.Name);
                return requestedAssembly.Name != "SlimDX" ? null : Assembly.LoadFrom(DXAsm);
            };

            AppDomain.CurrentDomain.AssemblyResolve += DXHandlerAsm;
        }

        #endregion
    }
}