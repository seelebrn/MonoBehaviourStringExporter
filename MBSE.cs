using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using HarmonyLib;
using System.Reflection;
using System.IO;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using TMPro;

namespace MBSE
{

    [BepInPlugin("Cadenza.MBSE.MOD", "MBSE", "0.5")]
    public class Plugin : BaseUnityPlugin
    {
        public static string[] forbidden = new string[] { "Sprite", "AnimationClip", "RuntimeAnimatorController", "Texture2D", "PlaceIcon", "AudioClip", "Material", "SpriteAtlas", "Font", "Shader", "TMP_FontAsset" };




        private void Awake()
        {
            Debug.Log("Welcome to MBSE");
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
                    //Debug.Log("Bundle Name =" + ab.name);
                    UnityEngine.Object[] objarr = bundle.LoadAllAssets<UnityEngine.Object>();

                    foreach (var x in objarr)
                    {
                        try
                        {
                            if (!Plugin.forbidden.Contains(x.GetType().Name))
                            {
                                List<string> list = new List<string>();

                                var y = UnityEngine.JsonUtility.ToJson(x, true);

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
                                                                                Debug.Log("SubValue = " + prop2.Value.ToString());
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
                        }
                        catch
                        {

                        }
                    }
                    List<GameObject> gameObjects = Resources.FindObjectsOfTypeAll<GameObject>().ToList();
                    foreach (GameObject go in gameObjects)
                    {
                        List<string> list = new List<string>();
                        TextMeshProUGUI[] tmp = go.GetComponentsInChildren<TextMeshProUGUI>();
                        UnityEngine.UI.Text[] tmp2 = go.GetComponentsInChildren<UnityEngine.UI.Text>();
                        tmp.AddRangeToArray<TextMeshProUGUI>(go.GetComponents<TextMeshProUGUI>());
                        tmp.AddRangeToArray<TextMeshProUGUI>(go.GetComponentsInParent<TextMeshProUGUI>());
                        tmp2.AddRangeToArray<UnityEngine.UI.Text>(go.GetComponents<UnityEngine.UI.Text>());
                        tmp2.AddRangeToArray<UnityEngine.UI.Text>(go.GetComponentsInParent<UnityEngine.UI.Text>());

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

    public static class Helpers
    {
        public static readonly Regex cjkCharRegex = new Regex(@"\p{IsCJKUnifiedIdeographs}");
        public static bool IsChinese(string s)
        {
            return cjkCharRegex.IsMatch(s);
        }
    }
}