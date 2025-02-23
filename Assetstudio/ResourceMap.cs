using MessagePack;
using System;
using System.Collections.Generic;
using System.IO;

namespace AssetStudio
{
    public static class ResourceMap
    {
        private static AssetMap Instance = new() { GameType = GameType.正常, AssetEntries = new List<AssetEntry>() };
        public static List<AssetEntry> GetEntries() => Instance.AssetEntries;
        public static void FromFile(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                Logger.Info(string.Format("解析...."));
                try
                {
                    using var stream = File.OpenRead(path);
                    Instance = MessagePackSerializer.Deserialize<AssetMap>(stream, MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4BlockArray));
                }
                catch (Exception e)
                {
                    Logger.Error("资源映射未加载");
                    Console.WriteLine(e.ToString());
                    return;
                }
                Logger.Info("加载!!");
            }
        }

        public static void Clear()
        {
            Instance.GameType = GameType.正常;
            Instance.AssetEntries = new List<AssetEntry>();
        }
    }
}
