using Core;
using Newtonsoft.Json;

namespace PdfFlipBook.Models
{
    public class BookFolder : ObservableObject
    {
        [JsonIgnore] public string Title { get; set; }
        [JsonIgnore] public string Icon { get; set; }

        [JsonIgnore]
        public bool IsDragging
        {
            get => GetOrCreate<bool>();
            set => SetAndNotify(value);
        }
    }
}