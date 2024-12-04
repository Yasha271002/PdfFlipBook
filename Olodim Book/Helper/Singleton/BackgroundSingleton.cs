using Core;

namespace PdfFlipBook.Helper.Singleton
{
    public class BackgroundSingleton:ObservableObject
    {
        private static readonly BackgroundSingleton _instance = new BackgroundSingleton();

        public static BackgroundSingleton Instance => _instance;

        public string? Background
        {
            get => GetOrCreate<string>();
            set => SetAndNotify(value);
        }

        private BackgroundSingleton()
        {
        }
    }
}