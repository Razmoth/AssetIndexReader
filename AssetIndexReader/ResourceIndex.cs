namespace AssetIndexReader
{
    public class ResourceIndex
    {
        public Dictionary<int, List<int>> BundleDependencyMap;
        public Dictionary<int, Block> BlockInfoMap;
        public Dictionary<int, byte> BlockMap;
        public Dictionary<int, ulong> AssetMap;
        public List<Dictionary<int, BundleInfo>> AssetLocationMap;
        public List<int> BlockSortList;
        public ResourceIndex()
        {
            BlockSortList = new List<int>();
            AssetMap = new();
            AssetLocationMap = new(0x100);
            for (int i = 0; i < AssetLocationMap.Capacity; i++)
            {
                AssetLocationMap.Add(new(0x1FF));
            }
            BundleDependencyMap = new();
            BlockInfoMap = new();
            BlockMap = new();
        }
        public void Clear()
        {
            BundleDependencyMap.Clear();
            BlockInfoMap.ToList().Clear();
            AssetLocationMap.ToList().ForEach(x => x.Clear());
            BlockMap.Clear();
        }
        public List<BundleInfo> GetAllAssets()
        {
            var hashes = new List<BundleInfo>();
            for (int i = 0; i < AssetLocationMap.Capacity; i++)
            {
                foreach(var pair in AssetLocationMap[i])
                {
                    hashes.Add(pair.Value);
                }
            }
            return hashes;
        }
        public List<BundleInfo> GetAssets(int bundle)
        {
            var hashes = new List<BundleInfo>();
            for (int i = 0; i < AssetLocationMap.Capacity; i++)
            {
                foreach (var pair in AssetLocationMap[i])
                {
                    if (pair.Value.Bundle == bundle)
                        hashes.Add(pair.Value);
                }
            }
            return hashes;
        }
        public BundleInfo GetBundleInfo(ulong hash)
        {
            var asset = new Asset(hash);
            if (AssetLocationMap.ElementAtOrDefault(asset.Pre) != null)
            {
                if (AssetLocationMap[asset.Pre].TryGetValue(asset.Last, out var bundleInfo)) return bundleInfo;
            }
            return null;
        }
        public BundleInfo GetBundleInfo(int last)
        {
            var hash = GetAssetIndex(last);
            var asset = new Asset(hash);
            if (AssetLocationMap.ElementAtOrDefault(asset.Pre) != null)
            {
                if (AssetLocationMap[asset.Pre].TryGetValue(asset.Last, out var bundleInfo)) return bundleInfo;
            }
            return null;
        }
        public ulong GetAssetIndex(int container)
        {
            if (AssetMap.TryGetValue(container, out var value)) return value;
            else return 0;
        }
        public List<int> GetAllAssetIndices(int bundle)
        {
            var hashes = new List<int>();
            for (int i = 0; i < AssetLocationMap.Capacity; i++)
            {
                foreach (var pair in AssetLocationMap[i])
                {
                    if (pair.Value.Bundle == bundle)
                        hashes.Add(pair.Key);
                }
            }
            return hashes;
        }
        public List<int> GetBundles(int id)
        {
            var bundles = new List<int>();
            foreach (var block in BlockInfoMap)
            {
                if (block.Value.Id == id)
                {
                    bundles.Add(block.Key);
                }
            }
            return bundles;
        }
        public Block GetBlockInfo(int bundle)
        {
            if (BlockInfoMap.TryGetValue(bundle, out var blk)) return blk;
            else return null;
        }
        public BlockFile GetBlockFile(int id)
        {
            if (BlockMap.TryGetValue(id, out var languageCode))
            {
                return new BlockFile(languageCode, id);
            }
            return null;
        }
        public int GetBlockID(int bundle)
        {
            if (BlockInfoMap.TryGetValue(bundle, out var block))
            {
                return block.Id;
            }
            else return 0;
        }
        public List<int> GetBundleDep(int bundle)
        {
            if (BundleDependencyMap.TryGetValue(bundle, out var dep)) return dep;
            else return new();
        }
        public bool CheckIsLegitAssetPath(ulong hash)
        {
            var asset = new Asset(hash);
            return AssetLocationMap[asset.Pre].ContainsKey(asset.Last);
        }
        public void AddAssetLocation(BundleInfo bundle, Asset asset)
        {
            AssetMap[asset.Last] = asset.Hash;
            AssetLocationMap[asset.Pre].Add(asset.Last, bundle);
        }
    }
}
