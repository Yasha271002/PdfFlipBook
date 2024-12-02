using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core;
using PdfFlipBook.Models;

namespace PdfFlipBook.ViewModel.Pages
{
    public class SelectBackgroundPageViewModel : ObservableObject
    {
        public List<ImageModel> ImageModel
        {
            get => GetOrCreate<List<ImageModel>>();
            set => SetAndNotify(value);
        }

        private string? _path = "Background";

        public SelectBackgroundPageViewModel()
        {
            ImageModel = new List<ImageModel>();
            GetImages();
        }

        private void GetImages()
        {
            if (Directory.Exists(_path))
                Directory.CreateDirectory(_path);

            var files = Directory.GetFiles(_path).ToList();

            foreach (var file in files)
            {
                ImageModel.Add(new ImageModel()
                {
                    ImagePath = Path.GetFullPath(file),
                });
            }
        }
    }
}