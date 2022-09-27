namespace AssetIndexReader
{
    public class BundleInfo
    {
        public int Bundle;
        public string Path;
        public BundleInfo(int bundle, string path)
        {
            Bundle = bundle;
            Path = path;
        }
    }

    public class Asset
    {
        public ulong Hash;
        public int Last => unchecked((int)(Hash >> 8));
        public byte Pre => (byte)(Hash & 0xFF);
        public Asset(ulong hash)
        {
            Hash = hash;
        }
    }

    public class Block
    {
        public int Id;
        public int Offset;

        public Block(int id, int offset)
        {
            Id = id;
            Offset = offset;
        }
    }

    public class BlockFile
    {
        public int LanguageCode;
        public int Id;
        public BlockFile(int languageCode, int id)
        {
            LanguageCode = languageCode;
            Id = id;
        }
        public string Path
        {
            get
            {
                var block = LanguageCode.ToString().PadLeft(2, '0');
                var blkName = Id.ToString().PadLeft(8, '0');
                var blkPath = string.Concat(block, "/", blkName, ".blk");
                var blockDirectory = "AssetBundles/blocks/";
                return blockDirectory + blkPath;
            }
        }
    }
}
