using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Modules.JS.Editor
{
    public class AddClassToConfig : UnityEngine.ScriptableObject
    {
        [UnityEditor.MenuItem("Assets/Puerts/Add To Config")]
        private static void Add()
        {
            var sc = UnityEditor.Selection.GetFiltered<UnityEditor.MonoScript>(UnityEditor.SelectionMode.Assets)?[0];
            var name = sc.GetClass().FullName;
            try
            {
                string s;
                string path = UnityEngine.Application.dataPath + "/Scripts/Modules/JS/Editor/ExportConfig.cs";
                using (System.IO.StreamReader sr = new System.IO.StreamReader(path, System.Text.Encoding.UTF8))
                {
                    s = sr.ReadToEnd();
                    int index = s.LastIndexOf("// TODO 添加TS使用的类");
                    if (index > -1)
                    {
                        s = s.Insert(index, $"typeof(global::{name}),\r\n\t\t\t\t\t");
                    }
                }
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(path))
                {
                    sw.Write(s);
                    sw.Flush();
                }
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogError(e);
            }

            UnityEditor.AssetDatabase.Refresh();
        }

        [UnityEditor.MenuItem("Assets/Puerts/Open Config")]
        private static void Open()
        {
            var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<UnityEditor.MonoScript>("Assets/Scripts/Modules/JS/Editor/ExportConfig.cs");

            int last = 0;
            int row = 0;
            int ln = asset.text.LastIndexOf("// TODO 添加TS使用的类");

            string flagRow = "\r\n";
            do
            {
                last = asset.text.IndexOf(flagRow, last) + 2;// 加4会略过空行 o.0
                row++;
            } while (last < ln);
            UnityEditor.AssetDatabase.OpenAsset(asset, row);
        }
    }

    [Puerts.Configure]
    public static class ExportConfig
    {
        /// <summary>
        /// 自定义导出的类
        /// </summary>
        [Puerts.Binding]
        private static IEnumerable<Type> Bindings => new List<Type>()
        {
            typeof(System.Collections.Generic.List<int>),
            typeof(System.Collections.Generic.Dictionary<int, List<int>>),
            // TODO 添加TS使用的类
        };

        /// <summary>
        /// Unity 引擎内置类
        /// 以及所有自定义的脚本
        /// </summary>
        [Puerts.Binding]
        private static IEnumerable<Type> DynamicBindings
        {
            get
            {
                var namespaces = new string[]
                {
                    "UnityEngine",
                    "UnityEngine.UI",
                    "FairyGUI",
                    "FairyGUI.Utils",
                };

                var types = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                                  where !(assembly.ManifestModule is ModuleBuilder)
                                  from type in assembly.GetExportedTypes()
                                  where type.Namespace != null
                                    && namespaces.Contains(type.Namespace)
                                    && !IsExcluded(type)
                                    && type.BaseType != typeof(MulticastDelegate)
                                    && !type.IsEnum
                                  select type);

                var customAssemblys = new string[]
                {
                    "Assembly-CSharp",
                };

                var customTypes = (from assembly in customAssemblys.Select(s => Assembly.Load(s))
                                   where !(assembly.ManifestModule is ModuleBuilder)
                                   from type in assembly.GetExportedTypes()
                                   where type.Namespace == null
                                    || !type.Namespace.StartsWith("Puerts")
                                    && !IsExcluded(type)
                                    && type.BaseType != typeof(MulticastDelegate)
                                    && !type.IsEnum
                                   select type);

                return types
                    .Concat(customTypes)
                    .Distinct();
            }
        }

        /// <summary>
        /// 判断是否需要被排除
        /// </summary>
        private static bool IsExcluded(Type type)
        {
            if (type == null) { return false; }

            var name = System.IO.Path.GetFileName(type.Assembly.Location);

            if (excludedAssemblies.Contains(name)) { return true; }

            var namespace_ = type.Namespace?.Replace('+', '.') ?? "";

            if (excludedNameSpaces.Contains(namespace_)) { return true; }

            var fullName = type.FullName?.Replace('+', '.') ?? "";

            if (excludedTypes.Contains(fullName)) { return true; }

            return IsExcluded(type.BaseType);
        }

        /// <summary>
        /// 优化GC，需要开启unsafe模式
        /// </summary>
        [Puerts.BlittableCopy]
        private static IEnumerable<Type> Blittables => new List<Type>()
        {
            typeof(UnityEngine.Vector2),
            typeof(UnityEngine.Vector3),
            typeof(UnityEngine.Vector4),
            typeof(UnityEngine.Rect),
        };

        /// <summary>
        /// 单独屏蔽类型内的某个方法
        /// </summary>
        [Puerts.Filter]
        private static bool Filter(MemberInfo memberInfo)
        {
            var filter = new List<(string, string)>()
            {
                ("MonoBehaviour", "runInEditMode"),
                ("MonoBehaviour", "OnRebuildRequested"),
                ("Graphic", "OnRebuildRequested"),
                ("Text", "OnRebuildRequested"),
                ("Texture2D", "alphaIsTransparency"),
                ("Texture", "imageContentsHash"),
                ("Input", "IsJoystickPreconfigured"),
            };
            //--------------------------------------------------------------
            // 添加要屏蔽的方法
            //--------------------------------------------------------------

            string t = memberInfo.DeclaringType.Name;
            string m = memberInfo.Name;

            foreach (var item in filter)
            {
                if (string.Equals(t, item.Item1) && string.Equals(m, item.Item2))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 需要排除的程序集
        /// </summary>
        private static readonly List<string> excludedAssemblies = new List<string>
        {
            "UnityEditor.dll",
            "Assembly-CSharp-Editor.dll",
		};

		private static readonly List<string> excludedNameSpaces = new List<string>
		{
        };

        /// <summary>
        /// 需要排除的类型
        /// </summary>
        private static readonly List<string> excludedTypes = new List<string>
        {
            "UnityEngine.iPhone",
            "UnityEngine.iPhoneTouch",
            "UnityEngine.iPhoneKeyboard",
            "UnityEngine.iPhoneInput",
            "UnityEngine.iPhoneAccelerationEvent",
            "UnityEngine.iPhoneUtils",
            "UnityEngine.iPhoneSettings",
            "UnityEngine.AndroidInput",
            "UnityEngine.AndroidJavaProxy",
            "UnityEngine.BitStream",
            "UnityEngine.ADBannerView",
            "UnityEngine.ADInterstitialAd",
            "UnityEngine.RemoteNotification",
            "UnityEngine.LocalNotification",
            "UnityEngine.NotificationServices",
            "UnityEngine.MasterServer",
            "UnityEngine.Network",
            "UnityEngine.NetworkView",
            "UnityEngine.ParticleSystemRenderer",
            "UnityEngine.ParticleSystem.CollisionEvent",
            "UnityEngine.ProceduralPropertyDescription",
            "UnityEngine.ProceduralTexture",
            "UnityEngine.ProceduralMaterial",
            "UnityEngine.ProceduralSystemRenderer",
            "UnityEngine.TerrainData",
            "UnityEngine.HostData",
            "UnityEngine.RPC",
            "UnityEngine.AnimationInfo",
            "UnityEngine.UI.IMask",
            "UnityEngine.Caching",
            "UnityEngine.Handheld",
            "UnityEngine.MeshRenderer",
            "UnityEngine.UI.DefaultControls",
            "UnityEngine.AnimationClipPair", //Obsolete
            "UnityEngine.CacheIndex", //Obsolete
            "UnityEngine.SerializePrivateVariables", //Obsolete
            "UnityEngine.Networking.NetworkTransport", //Obsolete
            "UnityEngine.Networking.ChannelQOS", //Obsolete
            "UnityEngine.Networking.ConnectionConfig", //Obsolete
            "UnityEngine.Networking.HostTopology", //Obsolete
            "UnityEngine.Networking.GlobalConfig", //Obsolete
            "UnityEngine.Networking.ConnectionSimulatorConfig", //Obsolete
            "UnityEngine.Networking.DownloadHandlerMovieTexture", //Obsolete
            "AssetModificationProcessor", //Obsolete
            "AddressablesPlayerBuildProcessor", //Obsolete
            "UnityEngine.WWW", //Obsolete
            "UnityEngine.EventSystems.TouchInputModule", //Obsolete
            "UnityEngine.MovieTexture", //Obsolete[ERROR]
            "UnityEngine.NetworkPlayer", //Obsolete[ERROR]
            "UnityEngine.NetworkViewID", //Obsolete[ERROR]
            "UnityEngine.NetworkMessageInfo", //Obsolete[ERROR]
            "UnityEngine.UI.BaseVertexEffect", //Obsolete[ERROR]
            "UnityEngine.UI.IVertexModifier", //Obsolete[ERROR]
            //Windows Obsolete[ERROR]
            "UnityEngine.EventProvider",
            "UnityEngine.UI.GraphicRebuildTracker",
            "UnityEngine.GUI.GroupScope",
            "UnityEngine.GUI.ScrollViewScope",
            "UnityEngine.GUI.ClipScope",
            "UnityEngine.GUILayout.HorizontalScope",
            "UnityEngine.GUILayout.VerticalScope",
            "UnityEngine.GUILayout.AreaScope",
            "UnityEngine.GUILayout.ScrollViewScope",
            "UnityEngine.GUIElement",
            "UnityEngine.GUILayer",
            "UnityEngine.GUIText",
            "UnityEngine.GUITexture",
            "UnityEngine.ClusterInput",
            "UnityEngine.ClusterNetwork",
            //System
            "System.Tuple",
            "System.Double",
            "System.Single",
            "System.ArgIterator",
            "System.SpanExtensions",
            "System.TypedReference",
            "System.StringBuilderExt",
            "System.IO.Stream",
            "System.Net.HttpListenerTimeoutManager",
            "System.Net.Sockets.SocketAsyncEventArgs",

            // TODO 添加要被排除的类
            "FairyGUI.TreeNode", // [obsolete]
            "FairyGUI.TreeView", // [obsolete]
        };
    }
}