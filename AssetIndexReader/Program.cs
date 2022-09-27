if (args.Length == 1)
{
    string inPath = File.Exists(args[0]) ? args[0] : null;
    if (string.IsNullOrEmpty(inPath))
    {
        Console.WriteLine("asset_index path is not valid !!");
        return;
    }

    var asset_index = new AssetIndex(inPath);
    asset_index.Read();
    var str = JsonConvert.SerializeObject(asset_index, Formatting.Indented);
    File.WriteAllText("./output.json", str);
}
else
{
    var versionString = Assembly.GetEntryAssembly()?
                                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                .InformationalVersion
                                .ToString();

    Console.WriteLine($"AssetIndexReader v{versionString}");
    Console.WriteLine("------------------------");
    Console.WriteLine("\nUsage:");
    Console.WriteLine("  AssetIndexReader <asset_index path>");
}