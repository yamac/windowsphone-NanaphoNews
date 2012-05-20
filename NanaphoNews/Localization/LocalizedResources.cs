namespace NanaphoNews.Localization
{
    public class LocalizedResources
    {
        public LocalizedResources()
        {
        }

        private static AppResources localizedResources = new AppResources();
        public AppResources Resources { get { return localizedResources; } }
    }
}
