using BepInEx;
using Fungus;
using HarmonyLib;
using MoonSharp.VsCodeDebugger.SDK;
using Newtonsoft.Json.Linq;
using script.Steam.UI.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Spine.Unity.Examples.SpineboyFootplanter;

namespace MBSE
{
    [BepInPlugin("Cadenza.MBSE.MOD", "MBSE", "0.5")]
    public class Plugin : BaseUnityPlugin
    {
        public static string[] forbidden = new string[] { "Sprite", "AnimationClip", "RuntimeAnimatorController", "Texture2D", "PlaceIcon", "AudioClip", "Material", "SpriteAtlas", "Font", "Shader", "TMP_FontAsset", "If", "SoftMaskable" };

        public static BepInEx.Logging.ManualLogSource log;
        public static List<Type> relevanttypes = new List<Type>();

        private void Awake()
        {
            log = Logger;
            log.LogInfo("Welcome to MBSE");
            Assembly startingassembly = Assembly.GetAssembly(typeof(Say));
            log.LogInfo("SA = " + startingassembly);

            foreach (Type t in startingassembly.GetTypes())
            {
                if (typeof(UnityEngine.Object).IsAssignableFrom(t) && !t.IsAbstract && !t.IsSealed && t.IsClass && !t.IsGenericType)
                {

                    //log.LogInfo("Type = " + t.Name);
                    relevanttypes.Add(t);
                }
            }
            relevanttypes.Add(typeof(UnityEngine.UI.Text));
            relevanttypes.Add(typeof(TextMeshProUGUI));





        }




        private void Update()
        {
            if (Input.GetKeyUp(KeyCode.F8) == true)
            {
                Dictionary<string, List<string>> listobj = new Dictionary<string, List<string>>();
                Dictionary<string, List<string>> listcomp = new Dictionary<string, List<string>>();

                if (File.Exists(Path.Combine(BepInEx.Paths.PluginPath, "MasterList.txt")))
                {
                    File.Delete(Path.Combine(BepInEx.Paths.PluginPath, "MasterList.txt"));
                }
                if (File.Exists(Path.Combine(BepInEx.Paths.PluginPath, "MasterListTMP.txt")))
                {
                    File.Delete(Path.Combine(BepInEx.Paths.PluginPath, "MasterListTMP.txt"));
                }
                if (!Directory.Exists(Path.Combine(BepInEx.Paths.PluginPath, "Assets")))
                {
                    Directory.CreateDirectory(Path.Combine(BepInEx.Paths.PluginPath, "Assets"));
                }

                System.IO.DirectoryInfo di = new DirectoryInfo(Path.Combine(BepInEx.Paths.PluginPath, "Assets"));

                foreach (FileInfo file in di.GetFiles())
                {
                    file.Delete();
                }

                List<string> cache0 = new List<string>();
                foreach (var obj in Resources.FindObjectsOfTypeAll(typeof(UnityEngine.Object)))
                {
                    if (relevanttypes.Contains(obj.GetType()) && !forbidden.Contains(obj.GetType().Name))
                    {
                        if (UnityEngine.JsonUtility.ToJson(obj) != null)
                        {

                            var deserialized = UnityEngine.JsonUtility.ToJson(obj);
                            if (deserialized != null)
                            {
                                var parsedStr = JObject.Parse(deserialized);
                                var query = parsedStr.Descendants().OfType<JProperty>().Where(p => p.Value.Type != JTokenType.Array && p.Value.Type != JTokenType.Object);
                                foreach (var property in query)

                                    if (Helpers.ContainsCH(query))
                                            {
                                        using (StreamWriter tw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "Assets", obj.GetType().Name + ".txt"), append: true))
                                        {
                                            foreach (string s in query)
                                            {
                                                if (Helpers.IsChinese(s) && !cache0.Contains(s))
                                                {
                                                    tw.Write(s + Environment.NewLine);
                                                }
                                                cache0.Add(s);
                                            }


                                            tw.Close();
                                        }
                                    }
                            }
                        }
                    }

                }

                List<string> cache = new List<string>();
                foreach (GameObject go in Resources.LoadAll<GameObject>(""))
                {
                    foreach (Type t in relevanttypes)
                    {
                        if (!forbidden.Contains(t.Name) && relevanttypes.Contains(t))
                        {
                            foreach (var y in go.GetComponentsInChildren(t, true))
                            {
                                var deserialized = UnityEngine.JsonUtility.ToJson(y);
                                var parsedStr = JObject.Parse(deserialized);
                                var query = parsedStr.Descendants().OfType<JProperty>().Where(p => p.Value.Type != JTokenType.Array && p.Value.Type != JTokenType.Object);
                                foreach (var property in query)
                                {

                                    using (StreamWriter tw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "Assets", y.GetType().Name + ".txt"), append: true))
                                    {

                                        if (Helpers.IsChinese(property.Value.ToString()) && !cache.Distinct().Contains(property.Value.ToString()))
                                        {
                                            tw.Write(property.Value.ToString() + Environment.NewLine);
                                        }


                                        tw.Close();
                                    }
                                    cache.Add(property.ToString());
                                }

                            }

                        }

                    }

                }















            }
        }
    }
}

public static class Helpers
{
    public static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
    public static bool IsChinese(string s)
    {
        return cjkCharRegex.IsMatch(s);
    }

    public static bool ContainsCH(IEnumerable<JProperty> list)
    {
        int flag = 0;

        foreach (var s in list)
        {
            if (Helpers.IsChinese(s.Value.ToString()))
            {
                flag = flag + 1;
            }


        }
        if (flag > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

}
