namespace Djambi.UI.Resources
{
    public static class ResourceFactory
    {
        public static IResourceService ResourceService { get; } = new HotdogTown.ResourceService();
    }
}
