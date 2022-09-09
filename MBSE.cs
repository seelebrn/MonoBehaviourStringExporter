using BepInEx;
using Fungus;
using HarmonyLib;
using Newtonsoft.Json.Linq;
using script.Steam.UI.Base;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Security.Policy;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace MBSE
{
    [BepInPlugin("Cadenza.MBSE.MOD", "MBSE", "0.5")]
    public class Plugin : BaseUnityPlugin
    {
        public static string[] forbidden = new string[] { "Sprite", "AnimationClip", "RuntimeAnimatorController", "Texture2D", "PlaceIcon", "AudioClip", "Material", "SpriteAtlas", "Font", "Shader", "TMP_FontAsset" };

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
                if (typeof(UnityEngine.Object).IsAssignableFrom(t))
                {
                    log.LogInfo("Type = " + t.Name);
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

                foreach (var bundle in AssetBundle.GetAllLoadedAssetBundles())
                {
                    UnityEngine.Object[] objarr = bundle.LoadAllAssets<UnityEngine.Object>();

                    /*foreach (var x in objarr)
                    {
                        if (!Plugin.forbidden.Contains(x.GetType().Name))
                        {
                            List<string> list = new List<string>();

                            var y = UnityEngine.JsonUtility.ToJson(x, true);
                            try
                            {
                                //Plugin.log.LogInfo("Y = " + y);
                                var p = JObject.Parse(y);
                                //Plugin.log.LogInfo("P = " + p);


                                if (p != null)
                                {
                                    Plugin.log.LogInfo("Non Null");
                                    foreach (var a in p.DescendantsAndSelf())
                                    {
                                        if (a is JObject obj)
                                            foreach (var prop in obj.Properties())
                                                if (!(prop.Value is JObject) && !(prop.Value is JArray))
                                                {
                                                    try
                                                    {
                                                        if (JObject.Parse(prop.Value.ToString()).HasValues)
                                                        {
                                                            var subjson = JObject.Parse(prop.Value.ToString());
                                                            foreach (var b in subjson.DescendantsAndSelf())
                                                            {
                                                                if (b is JObject obj2)
                                                                {
                                                                    foreach (var prop2 in obj2.Properties())
                                                                    {
                                                                        if (!(prop2.Value is JObject) && !(prop2.Value is JArray) && prop2.Value != null)
                                                                        {
                                                                            if (Helpers.IsChinese(prop2.Value.ToString()))
                                                                            {
                                                                                Plugin.log.LogInfo("SubValue = " + prop2.Value.ToString());
                                                                                list.Add(prop2.Value.ToString().Replace("\n", ""));
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }
                                                        }


                                                    }
                                                    catch
                                                    {

                                                    }
                                                    if (Helpers.IsChinese(prop.Value.ToString()) && !prop.Value.ToString().StartsWith("{"))
                                                    {
                                                        Plugin.log.LogInfo("Value = " + prop.Value.ToString().Replace("\n", ""));
                                                        list.Add(prop.Value.ToString().Replace("\n", ""));

                                                    }



                                                }



                                    }
                                }
                                else
                                {
                                    Plugin.log.LogInfo("Null");
                                }
                            }
                            catch
                            {

                            }



                            using (StreamWriter tw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "Assets", x.GetType().Name + x.GetHashCode() + ".txt"), append: true))
                            {

                                foreach (string s in list.Distinct())
                                {
                                    if (Helpers.IsChinese(s))
                                    {
                                        tw.Write(s + Environment.NewLine);
                                    }
                                }
                                tw.Close();
                            }
                            using (StreamWriter tw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "MasterList.txt"), append: true))
                            {

                                foreach (string s in list.Distinct())
                                {
                                    if (Helpers.IsChinese(s))
                                    {
                                        tw.Write(s + Environment.NewLine);
                                    }
                                }
                                tw.Close();
                            }

                        }
                    }*/


                    foreach (var go in Resources.LoadAll<GameObject>(""))
                    {
                        foreach (Type t in relevanttypes)
                        {
                            foreach (var fs in go.GetComponentsInChildren(t))
                            {
                                Plugin.log.LogInfo("Count 1= " + go.GetComponentsInChildren(t).Count());
                                //Plugin.log.LogInfo("Found the Component in: " + foundScript.gameObject);
                                List<string> list = new List<string>();




                                var y = UnityEngine.JsonUtility.ToJson(fs, true);

                                try
                                {

                                    //Debug.Log("Y = " + y);
                                    var p = JObject.Parse(y);
                                    //Debug.Log("P = " + p);


                                    if (p != null)
                                    {

                                        Debug.Log("Non Null");
                                        foreach (var a in p.DescendantsAndSelf())
                                        {
                                            if (a is JObject obj)
                                                foreach (var prop in obj.Properties())
                                                    if (!(prop.Value is JObject) && !(prop.Value is JArray))
                                                    {
                                                        try
                                                        {
                                                            if (Helpers.IsChinese(prop.Value.ToString()))
                                                            {
                                                                list.Add(prop.Value.ToString().Replace("\n", ""));
                                                            }
                                                            if (Helpers.IsChinese(Regex.Unescape(prop.Value.ToString())))
                                                            {
                                                                list.Add(Regex.Unescape(prop.Value.ToString()));
                                                            }
                                                            if (JObject.Parse(prop.Value.ToString()).HasValues)
                                                            {
                                                                var subjson = JObject.Parse(prop.Value.ToString());
                                                                foreach (var b in subjson.DescendantsAndSelf())
                                                                {
                                                                    if (b is JObject obj2)
                                                                    {
                                                                        foreach (var prop2 in obj2.Properties())
                                                                        {
                                                                            if (!(prop2.Value is JObject) && !(prop2.Value is JArray) && prop2.Value != null)
                                                                            {
                                                                                if (Helpers.IsChinese(prop2.Value.ToString()))
                                                                                {
                                                                                    list.Add(prop2.Value.ToString().Replace("\n", ""));
                                                                                }
                                                                                if (Helpers.IsChinese(Regex.Unescape(prop2.Value.ToString())))
                                                                                {
                                                                                    list.Add(Regex.Unescape(prop2.Value.ToString()));
                                                                                }
                                                                            }
                                                                        }
                                                                    }
                                                                }
                                                            }


                                                        }
                                                        catch
                                                        {

                                                        }
                                                        if (Helpers.IsChinese(prop.Value.ToString()) && !prop.Value.ToString().StartsWith("{"))
                                                        {
                                                            Debug.Log("Value = " + prop.Value.ToString().Replace("\n", ""));
                                                            list.Add(prop.Value.ToString().Replace("\n", ""));

                                                        }



                                                    }

                                        }

                                    }
                                    else
                                    {
                                        Debug.Log("Null");
                                    }
                                }
                                catch
                                {

                                }


                                if (list.Count > 0)
                                {

                                    using (StreamWriter tw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "Assets", fs.GetType().Name + ".txt"), append: true))
                                    {

                                        foreach (string s in list.Distinct())
                                        {
                                            if (Helpers.IsChinese(s))
                                            {
                                                tw.Write(s + Environment.NewLine);
                                            }
                                        }
                                        tw.Close();
                                    }
                                    using (StreamWriter tw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "MasterList.txt"), append: true))
                                    {

                                        foreach (string s in list.Distinct())
                                        {
                                            if (Helpers.IsChinese(s))
                                            {
                                                tw.Write(s + Environment.NewLine);
                                            }
                                        }
                                        tw.Close();
                                    }
                                }
                            }

                        }


                    }

                    System.Collections.Generic.List<GameObject> gameObjects = bundle.LoadAllAssets<GameObject>().ToList();

                    foreach (GameObject go in gameObjects)
                    {

                        System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
                        TextMeshProUGUI[] tmp = go.GetComponentsInChildren<TextMeshProUGUI>(true);
                        UnityEngine.UI.Text[] tmp2 = go.GetComponentsInChildren<UnityEngine.UI.Text>(true);
                        tmp.AddRangeToArray(go.GetComponents<TextMeshProUGUI>());
                        tmp.AddRangeToArray(go.GetComponentsInParent(typeof(TextMeshProUGUI), true));
                        tmp2.AddRangeToArray(go.GetComponents<UnityEngine.UI.Text>());
                        tmp2.AddRangeToArray(go.GetComponentsInParent(typeof(UnityEngine.UI.Text), true));


                        foreach (TextMeshProUGUI t in tmp)
                        {
                            if (!list.Contains(t.text.Replace("\n", "")))
                            {
                                list.Add(t.text.Replace("\n", ""));
                            }
                        }
                        foreach (UnityEngine.UI.Text t2 in tmp2)
                        {
                            {
                                if (!list.Contains(t2.text.Replace("\n", "")))
                                {
                                    list.Add(t2.text.Replace("\n", ""));
                                }
                            }

                        }
                        using (StreamWriter tw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "Assets", "GameObject" + go.GetHashCode() + ".txt"), append: true))
                        {

                            foreach (string s in list.Distinct())
                            {
                                if (Helpers.IsChinese(s))
                                {
                                    tw.Write(s + Environment.NewLine);
                                }
                            }
                            tw.Close();
                        }
                        using (StreamWriter tw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "MasterListTMP.txt"), append: true))
                        {

                            foreach (string s in list.Distinct())
                            {
                                if (Helpers.IsChinese(s))
                                {
                                    tw.Write(s + Environment.NewLine);
                                }
                            }
                            tw.Close();
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
}
