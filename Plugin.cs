using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;

namespace NoVoidDeath
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        private static ManualLogSource mls;
        public static LocationType TpLocationConfig;
        private void Awake()
        {
            // Plugin startup logic
            mls = BepInEx.Logging.Logger.CreateLogSource(PluginInfo.PLUGIN_GUID);
            string tpLocationConfig = Config.Bind("General", "TpLocationConfig", "Ship", new ConfigDescription("Where you get TPed if you fall out of bounds. If you usually die to inverse teleporter then Ship makes more sense, if it's another TP then entrance might make more sense to you.", new AcceptableValueList<string>(["None", "Ship", "Entrance"]))).Value;
            TpLocationConfig = tpLocationConfig switch
            {
                "Ship" => LocationType.Ship,
                "Entrance" => LocationType.Entrance,
                _ => LocationType.None,
            };
            Log("If we fall out of bound, we TP to: " + TpLocationConfig.ToString());
            
            if(TpLocationConfig != LocationType.None)
            {
                //If we don't TP then don't patch and mod is kinda useless but eh...
                var harmony = new Harmony("NoVoidDeath");
                harmony.PatchAll(typeof(OutOfBoundsTrigger_Patch));
            }

            Log($"Plugin {PluginInfo.PLUGIN_GUID} is loaded!");
        }

        public enum LocationType
        {
            None,
            Ship,
            Entrance
        }

        public enum LogType
        {
            Message,
            Warning,
            Error,
            Fatal,
            Debug
        }

        internal static void Log(string message, LogType type = LogType.Message)
        {
#if !DEBUG
            if (type == LogType.Debug) {
                mls.LogMessage(message);
                return;
            }
#endif
            switch (type)
            {
                case LogType.Warning: mls.LogWarning(message); break;
                case LogType.Error: mls.LogError(message); break;
                case LogType.Fatal: mls.LogFatal(message); break;
                default: mls.LogMessage(message); break;
            }
        }
    }
}
