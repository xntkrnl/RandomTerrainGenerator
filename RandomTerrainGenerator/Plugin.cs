using UnityEngine;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.Reflection;

namespace RandomTerrainGenerator
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        public const string modGUID = "mborsh.RandomTerrainGenerator";
        public const string modName = "RandomTerrainGenerator";
        public const string modVersion = "0.0.0";

        public static Plugin Instance = null!;
        private ManualLogSource mls = null!;

        internal static readonly Harmony harmony = new Harmony(modGUID);

        private void Awake()
        {
            if (Instance == null)
                Instance = this;

            mls = Logger;

            NetcodePatcher();
        }

        internal static void Log(string message)
        {
            if (Instance) Instance.mls.LogInfo(message); //i need this for unity editor
            else Debug.Log(message);
        }

        private static void NetcodePatcher()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }
    }
}