using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Navigation;
using Core;
using PdfFlipBook.Views.Pages;
using WPFMitsuControls;

namespace PdfFlipBook.Helper
{
    public enum PageTypes
    {
        StartPage,
        RazdelPage,
        BookPage,
        SelectBackground,
        None
    }
  

    public class NavigationManager : ObservableObject
    {
        public static NavigationManager Instance { get; } = new NavigationManager();

        public static NavigationService Frame1
        {
            get => Instance.GetOrCreate<NavigationService>();
            set => Instance.SetAndNotify(value, callback: FrameChangedCallback);
        }

        public static NavigationService PopupFrame
        {
            get => Instance.GetOrCreate<NavigationService>();
            set => Instance.SetAndNotify(value);
        }

        public PageTypes CurrentPage
        {
            get => GetOrCreate<PageTypes>();
            set => SetAndNotify(value);
        }

        public bool IsPopupOpen
        {
            get => GetOrCreate<bool>();
            set => SetAndNotify(value);
        }

        private static void FrameChangedCallback(PropertyChangingArgs<NavigationService> obj)
        {
            if (obj.OldValue is not null)
                obj.OldValue.Navigated -= OnNavigated;

            if (obj.NewValue is not null)
                obj.NewValue.Navigated += OnNavigated;
        }

        private static void OnNavigated(object sender, NavigationEventArgs e)
        {
            Instance.CurrentPage = GetCurrentPage();
        }

        private static PageTypes GetCurrentPage()
        {
            return Frame1.Content switch
            {
                Start_Page => PageTypes.StartPage,
                Razdel_Page => PageTypes.RazdelPage,
                Book_Page => PageTypes.BookPage,
                SelectBackgroundPage => PageTypes.SelectBackground,
                _ => PageTypes.None
            };
        }

        public static void ClosePopup()
        {
            Instance.IsPopupOpen = false;

            Task.Run(async () =>
            {
                await Task.Delay(500); //Animation

                Application.Current.Dispatcher.Invoke(() =>
                {
                    if (!Instance.IsPopupOpen)
                        PopupFrame?.Navigate(null);
                });
            });
        }

        public static void ClearHistory()
        {
            while (Frame1.CanGoBack)
            {
                try
                {
                    Frame1.RemoveBackEntry();
                }
                catch (Exception ex)
                {
                    break;
                }
            }
        }
    }
}