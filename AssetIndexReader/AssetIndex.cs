using System.Text;

namespace AssetIndexReader
{
    public class AssetIndex : ResourceIndex
    {
        private BinaryReader reader;
        private int bundleCount;
        public Dictionary<string, string> AssetTypes = new();
        public List<int> PreloadBlocks = new();
        public List<int> PreloadShaderBlocks = new();

        public string ReadString()
        {
            var count = reader.ReadInt32();
            var str = reader.ReadBytes(count);
            return Encoding.UTF8.GetString(str);
        }

        public AssetIndex(string path)
        {
            var stream = File.OpenRead(path);
            reader = new BinaryReader(stream);
        }

        public void Read()
        {
            ReadAssetTypeMap();
            ReadAssetInfoMap();
            ReadBundleInfoMap();
            ReadPreloadBlockMap();
            ReadPreloadShaderBlockMap();
            ReadBlockMap();
            ReadBlockInfoMap();
            ReadBlockSortMap();
        }

        public void ReadAssetTypeMap()
        {
            var assetTypeCount = reader.ReadInt32();
            AssetTypes = new Dictionary<string, string>(assetTypeCount);
            for (int i = 0; i < assetTypeCount; i++)
            {
                var key = ReadString();
                var value = ReadString();
                AssetTypes.Add(key, value);
            }
        }

        public void ReadAssetInfoMap()
        {
            var count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var bundle = reader.ReadInt32();
                var hash = reader.ReadUInt64();
                var path = ReadString();
                AddAssetLocation(new(bundle, path), new(hash));
            }
        }
        public void ReadBundleInfoMap()
        {
            var bundleInfoCount = reader.ReadInt32();
            BundleDependencyMap.EnsureCapacity(bundleInfoCount);
            for (int i = 0; i < bundleInfoCount; i++)
            {
                var bundle = reader.ReadInt32();
                bundleCount = Math.Max(bundle + 1, bundleCount);
                var count = reader.ReadInt32();
                var dependencies = new List<int>(count);
                for (int j = 0; j < count; j++)
                {
                    var dependency = reader.ReadInt32();
                    dependencies.Add(dependency);
                    bundleCount = Math.Max(dependency + 1, bundleCount);
                }
                BundleDependencyMap.Add(bundle, dependencies);
            }
        }
        public void ReadPreloadBlockMap()
        {
            var preloadBlockCount = reader.ReadInt32();
            for (int i = 0; i < preloadBlockCount; i++)
            {
                PreloadBlocks.Add(reader.ReadInt32());
            }
        }
        public void ReadPreloadShaderBlockMap()
        {
            var preloadShaderBlockCount = reader.ReadInt32();
            for (int i = 0; i < preloadShaderBlockCount; i++)
            {
                PreloadShaderBlocks.Add(reader.ReadInt32());
            }
        }
        public void ReadBlockMap()
        {
            var blockInfoCount = reader.ReadInt32();
            for (int i = 0; i < blockInfoCount; i++)
            {
                var block = reader.ReadInt32();
                var blkCount = reader.ReadInt32();
                for (int j = 0; j < blkCount; j++)
                {
                    var blk = reader.ReadInt32();
                    BlockMap.Add(blk, (byte)block);
                }
            }
        }

        public void ReadBlockInfoMap()
        {
            var count = reader.ReadInt32();
            BlockInfoMap.EnsureCapacity(bundleCount);
            for (int i = 0; i < count; i++)
            {
                var id = reader.ReadInt32();
                var blockCount = reader.ReadInt32();
                for (int j = 0; j < blockCount; j++)
                {
                    var bundle = reader.ReadInt32();
                    var offset = reader.ReadInt32();
                    BlockInfoMap.Add(bundle, new(id, offset));
                }
            }
        }

        public void ReadBlockSortMap()
        {
            var blockSortCount = reader.ReadInt32();
            for (int i = 0; i < blockSortCount; i++)
            {
                var blk = reader.ReadInt32();
                BlockSortList.Add(blk);
            }
        }
    }
}
