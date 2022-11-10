using System;
using Godot;
using Newtonsoft.Json;

namespace GodotXUnitApi.Internal
{
    public static class WorkFiles
    {
        public static readonly JsonSerializerSettings jsonSettings = new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public const string WorkDir = "res://addons/GodotXUnit/_work";

        public static string PathForResFile(string filename)
        {
            var appending = filename.EndsWith(".json") ? filename : $"{filename}.json";
            return $"{WorkDir}/{appending}";
        }

        public static void CleanWorkDir()
        {
            using (var directory = DirAccess.Open(WorkDir))
            {
                directory.ListDirBegin();
                while (true)
                {
                    var next = directory.GetNext();
                    if (string.IsNullOrEmpty(next))
                        break;
                    directory.Remove(next);
                }
                directory.ListDirEnd();
            }
        }

        public static void WriteFile(string filename, object contents)
        {
            var writing = JsonConvert.SerializeObject(contents, Formatting.Indented, jsonSettings);
            using (var file = FileAccess.Open(PathForResFile(filename), FileAccess.ModeFlags.WriteRead))
            {
                file.StoreString(writing);
            }
        }

        public static object ReadFile(string filename)
        {
            using (var file = FileAccess.Open(PathForResFile(filename), FileAccess.ModeFlags.Read))
            {
                return JsonConvert.DeserializeObject(file.GetAsText(), jsonSettings);    
            }
        }
    }
}