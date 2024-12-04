using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using PdfFlipBook.Helper;
using PdfFlipBook.Helper.Singleton;
using PdfFlipBook.Models;
using ICommand = System.Windows.Input.ICommand;

namespace PdfFlipBook.ViewModel.Pages
{
    public class SelectBackgroundPageViewModel : ObservableObject
    {
        public List<ImageModel> ImageModel
        {
            get => GetOrCreate<List<ImageModel>>();
            set => SetAndNotify(value);
        }

        private readonly JsonHelper _jsonHelper;
        private string? _path = "background.json";
        private string? _directoryPath = "Backgrounds";

        public SelectBackgroundPageViewModel()
        {
            _jsonHelper = new JsonHelper();
            ImageModel = new List<ImageModel>();
            GetImages();
        }

        private void GetImages()
        {
            if (Directory.Exists(_directoryPath))
                Directory.CreateDirectory(_directoryPath);

            var files = Directory.GetFiles(_directoryPath).ToList();

            foreach (var file in files)
            {
                ImageModel.Add(new ImageModel()
                {
                    ImagePath = Path.GetFullPath(file),
                });
            }
        }

        public ICommand SaveBackgroundCommand => GetOrCreate(new RelayCommand(f =>
        {
            var selectedImage = ImageModel.FirstOrDefault(s => s.IsSelected == true);

            if (selectedImage == null) return;

            BackgroundSingleton.Instance.Background = Path.GetFullPath(selectedImage.ImagePath!);
            _jsonHelper.WriteJsonToFile(_path, Path.GetFullPath(selectedImage.ImagePath!), false);
        }));
    }
}