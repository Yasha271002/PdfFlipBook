using System.Collections.Generic;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using PdfFlipBook.Models;
using PdfFlipBook.ViewModel.Pages;
using PdfFlipBook.Views.Pages;
using RelayCommand = Core.RelayCommand;
using System.Collections.ObjectModel;

namespace PdfFlipBook.Helper
{

    public static class CommonCommands
    {
        private static Page? GetPageByType(PageTypes pageType)
        {
            return pageType switch
            {
                PageTypes.None => null,
                _ => null
            };
        }

        private static Page? GetPageByContent(object content)
        {
            Page? page = content switch
            {
                SettingsModel settings => new SettingsPage{DataContext = new SettingsPageViewModel(settings)},
                Tuple<string, List<BookPDF>, SettingsModel> razdelData => new Razdel_Page(razdelData.Item1, razdelData.Item2, razdelData.Item3),
                Tuple<string, SettingsModel> bookData => new Book_Page(bookData.Item1, bookData.Item2),
                _ => null
            };
            return page;
        }

        public static ICommand NavigateCommand { get; } = new RelayCommand(f =>
        {
            Page? page;
            if (f is PageTypes pageType)
            {
                page = GetPageByType(pageType);
            }
            else
            {
                page = GetPageByContent(f);
            }

            if (page == null) return;
            NavigationManager.Frame1.Navigate(page);

        });

        public static ICommand? GoBackCommand { get; } =
            new RelayCommand(f => { NavigationManager.Frame1.GoBack(); });

    }
}