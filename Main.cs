using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using BepInEx.IL2CPP;
using HarmonyLib;
using System.Reflection;
using System.IO;
using Il2CppSystem;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using UnhollowerBaseLib;
using Il2CppSystem.Linq;
using UnhollowerRuntimeLib;
using Newtonsoft.Json.Linq;
using TMPro;
using Il2CppSystem.Collections.Generic;

namespace MBSE
{
    [BepInPlugin("Cadenza.MBSE.MOD", "MBSE", "0.5")]
    public class Plugin : BasePlugin
    {
        public static string[] forbidden = new string[] { "Sprite", "AnimationClip", "RuntimeAnimatorController", "Texture2D", "PlaceIcon", "AudioClip", "Material", "SpriteAtlas", "Font", "Shader", "TMP_FontAsset" };

        public static BepInEx.Logging.ManualLogSource log;
        public static System.Collections.Generic.List<Il2CppSystem.Type> relevanttypes = new System.Collections.Generic.List<Il2CppSystem.Type>();

        public override void Load()
        {
            AddComponent<mbmb>();
            log = Log;
            log.LogInfo("Welcome to MBSE");
            Il2CppSystem.Reflection.Assembly startingassembly = Il2CppSystem.Reflection.Assembly.GetAssembly(Il2CppType.Of<MapUI>());
            log.LogInfo("SA = " + startingassembly);

            foreach (Il2CppSystem.Type t in startingassembly.GetTypes())
            {
                if (Il2CppType.Of<UnityEngine.Object>().IsAssignableFrom(t))
                {
                    log.LogInfo("Type = " + t.Name);
                    relevanttypes.Add(t);
                }
            }
            relevanttypes.Add(Il2CppType.Of<UnityEngine.UI.Text>());
            relevanttypes.Add(Il2CppType.Of<TextMeshProUGUI>());





        }

    }


    class mbmb : MonoBehaviour
    {
        public mbmb(System.IntPtr handle) : base(handle) { }
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
                foreach (var bundle in AssetBundle.GetAllLoadedAssetBundles_Native())
                {
                    UnityEngine.Object[] objarr = bundle.LoadAllAssets<UnityEngine.Object>();

                    foreach (var x in objarr)
                    {
                        if (!Plugin.forbidden.Contains(x.GetIl2CppType().Name))
                        {
                            System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();

                            var y = UnityEngine.JsonUtility.ToJsonInternal(x, true);
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
                                        tw.Write(s + Il2CppSystem.Environment.NewLine);
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
                                        tw.Write(s + Il2CppSystem.Environment.NewLine);
                                    }
                                }
                                tw.Close();
                            }

                        }
                    }

                    var gos = Resources.LoadAll("", GameObject.Il2CppType);
                    Plugin.log.LogInfo("GOS count = " + gos.OfType<GameObject>());
                    foreach (var go in gos.OfType<GameObject>())
                    {
                        Plugin.log.LogInfo("Go = " + go.name);
                        foreach (Il2CppSystem.Type t in Plugin.relevanttypes)
                        {
                            foreach (var fs in go.GetComponentsInChildren(t))
                            {
                                Plugin.log.LogInfo("Count 1= " + go.GetComponentsInChildren(t).Count());
                                //Plugin.log.LogInfo("Found the Component in: " + foundScript.gameObject);
                                System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();


                                try
                                {

                                    var y = UnityEngine.JsonUtility.ToJson(fs, true);



                                    //Debug.Log("Y = " + y);
                                    var p = JObject.Parse(y);
                                    //Debug.Log("P = " + p);


                                    if (p != null)
                                    {

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
                                                                                if (Helpers.IsChinese(prop2.Value.ToString()) && !prop.Value.ToString().StartsWith("{"))
                                                                                {
                                                                                    list.Add(prop2.Value.ToString().Replace("\n", ""));
                                                                                }
                                                                                if (Helpers.IsChinese(Regex.Unescape(prop2.Value.ToString())) && !prop.Value.ToString().StartsWith("{"))
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

                                    if (list.Count > 0)
                                    {

                                        using (StreamWriter tw = new StreamWriter(Path.Combine(BepInEx.Paths.PluginPath, "Assets", fs.GetIl2CppType().Name + ".txt"), append: true))
                                        {

                                            foreach (string s in list.Distinct())
                                            {
                                                if (Helpers.IsChinese(s))
                                                {
                                                    tw.Write(s + Il2CppSystem.Environment.NewLine);
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
                                                    tw.Write(s + Il2CppSystem.Environment.NewLine);
                                                }
                                            }
                                            tw.Close();
                                        }
                                    }
                                }
                                catch
                                {

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
                            tmp.AddRangeToArray<TextMeshProUGUI>(go.GetComponents<TextMeshProUGUI>());
                            tmp.AddRangeToArray<Component>(go.GetComponentsInParent(TextMeshProUGUI.Il2CppType, true));
                            tmp2.AddRangeToArray<UnityEngine.UI.Text>(go.GetComponents<UnityEngine.UI.Text>());
                            tmp2.AddRangeToArray<Component>(go.GetComponentsInParent(UnityEngine.UI.Text.Il2CppType, true));

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
                                        tw.Write(s + Il2CppSystem.Environment.NewLine);
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
                                        tw.Write(s + Il2CppSystem.Environment.NewLine);
                                    }
                                }
                                tw.Close();
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
}
