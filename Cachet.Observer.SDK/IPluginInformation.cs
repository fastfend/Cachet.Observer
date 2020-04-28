namespace CachetObserver.SDK
{
    public interface IPluginInformation
    {
        string Name { get; }
        string Description { get; }
        string Author { get; }
        string Version { get; }
        SupportedSDKVersion SupportedSDK { get; }
    }
}
