/*
DeepDungeon is licensed under a
Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International License.

You should have received a copy of the license along with this
work. If not, see <http://creativecommons.org/licenses/by-nc-sa/4.0/>.

Orginal work done by zzi, contibutions by Omninewb, Freiheit, and mastahg
                                                                                 */

using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using DeepCombined.Helpers.Logging;
using DeepCombined.Memory.Attributes;
using ff14bot;
using ff14bot.Enums;
using ff14bot.Helpers;
using GreyMagic;
using Newtonsoft.Json;

namespace DeepCombined.Memory
{
    
    internal class OffsetManager
    {
        private static string OffsetFile => Path.Combine(JsonSettings.SettingsPath, $"DeepCombined_Offsets_{Core.CurrentGameVer}.json");
        public static ConcurrentDictionary<string, long> OffsetCache = new ConcurrentDictionary<string, long>();
        
        
        
        
        internal static void Init()
        {
            var types = typeof(Offsets).GetFields(BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            
            if (File.Exists(OffsetFile))
            {
                OffsetCache = JsonConvert.DeserializeObject<ConcurrentDictionary<string, long>>(File.ReadAllText(OffsetFile));
                if (OffsetCache == null)
                {
                    OffsetCache = new ConcurrentDictionary<string, long>();
                }
            }
            
            
            
            bool foundAll = true;
            foreach (var type in types)
            {
                if (type.FieldType.IsClass)
                {
                    Logger.Error("Parsing class {0}, this shouldn't happen", type.FieldType.Name);

                }
                else
                {
                    var name = $"{type.DeclaringType?.FullName}.{type.Name}";
                    Logger.Info("Parsing field {0}", type.Name);
                    
                    if (OffsetCache.TryGetValue(name, out var offsetVal))
                    {
                        if (type.FieldType == typeof(IntPtr))
                        {
                            Logger.Info("Offset found in cache: {0}", Core.Memory.GetAbsolute(new IntPtr(offsetVal)).ToString("X"));
                            type.SetValue(null, Core.Memory.GetAbsolute(new IntPtr(offsetVal)));
                        }
                        else
                        {
                            Logger.Info("Offset found in cache: {0}", offsetVal);
                            type.SetValue(null, (int)offsetVal);
                        }
                        continue;
                    }

                    foundAll = false;
                }
            }
            
            if (foundAll)
            {
                return;
            }
            
            using var pf = new PatternFinder(Core.Memory);
            
            Parallel.ForEach(types, type =>
                {
                    if (type.FieldType.IsClass)
                    {
                        Logger.Info("Parsing class {0}", type.FieldType.Name);
                        var instance = Activator.CreateInstance(type.FieldType);
            

                        foreach (var field in type.FieldType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance))
                        {
                            var res = ParseField(field, pf);
                            if (field.FieldType == typeof(IntPtr))
                                field.SetValue(instance, res);
                            else
                                field.SetValue(instance, (int)res);
                        }

                        //set the value
                        type.SetValue(null, instance);
                    }
                    else
                    {
                        Logger.Info("Parsing field {0}", type.Name);
                        var res = ParseField(type, pf);
                        if (type.FieldType == typeof(IntPtr))
                            type.SetValue(null, res);
                        else
                            type.SetValue(null, (int)res);
                    }

                }
            );
            
            File.WriteAllText(OffsetFile, JsonConvert.SerializeObject(OffsetCache));
        }

        private static IntPtr ParseField(FieldInfo field, PatternFinder pf)
        {
            var offset = (OffsetAttribute)Attribute.GetCustomAttributes(field, typeof(OffsetAttribute))
                .FirstOrDefault();
            var valcn = (OffsetValueCN)Attribute.GetCustomAttributes(field, typeof(OffsetValueCN))
                .FirstOrDefault();
            var valna = (OffsetValueNA)Attribute.GetCustomAttributes(field, typeof(OffsetValueNA))
                .FirstOrDefault();

            var result = IntPtr.Zero;

            if (Constants.Lang == Language.Chn)
            {
                if (valcn != null)
                    return (IntPtr)valcn.Value;
                if (offset == null) return IntPtr.Zero;

                try
                {
                    result = pf.FindSingle(offset != null ? offset.PatternCN : offset.Pattern, true);
                    if (result != IntPtr.Zero)
                    {
                        if (field.FieldType != typeof(int))
                        {
                            OffsetCache.TryAdd($"{field.DeclaringType?.FullName}.{field.Name}", Core.Memory.GetRelative(result).ToInt64());
                        }
                        else
                        {
                            OffsetCache.TryAdd($"{field.DeclaringType?.FullName}.{field.Name}", result.ToInt64());
                        }
                    }
                }
                catch (Exception e)
                {
                    if (field.DeclaringType != null && field.DeclaringType.IsNested)
                        Logger.Error($"[{field.DeclaringType.DeclaringType.Name}:{field.Name:,27}] Not Found");
                    else
                        Logger.Error($"[{field.DeclaringType.Name}:{field.Name:,27}] Not Found");
                }
            }
            else
            {
                if (valna != null)
                    return (IntPtr)valna.Value;
                if (offset == null) return IntPtr.Zero;

                try
                {
                    //Logger.Information($"Not found in cache : {field.DeclaringType.FullName}.{field.Name}");
                    result = pf.FindSingle(offset.Pattern, true);
                    if (result != IntPtr.Zero)
                    {
                        if (field.FieldType != typeof(int))
                        {
                            OffsetCache.TryAdd($"{field.DeclaringType?.FullName}.{field.Name}", Core.Memory.GetRelative(result).ToInt64());
                        }
                        else
                        {
                            OffsetCache.TryAdd($"{field.DeclaringType?.FullName}.{field.Name}", result.ToInt64());
                        }
                    }
                }
                catch (Exception e)
                {
                    if (field.DeclaringType != null && field.DeclaringType.IsNested)
                        Logger.Error($"[{field.DeclaringType.DeclaringType.Name}:{field.Name:,27}] Not Found");
                    else
                        Logger.Error($"[{field.DeclaringType.Name}:{field.Name:,27}] Not Found");
                }
            }

            Logger.Info("[OffsetManager][{0:,27}] {1}", field.Name, result.ToString("X"));

            return result;
        }
    }
}