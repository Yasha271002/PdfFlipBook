using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core;

namespace PdfFlipBook.Models
{
    public class CountBooksModel:ObservableObject
    {
        public string Count
        {
            get => GetOrCreate<string>();
            set => SetAndNotify(value);
        }
    }
}
