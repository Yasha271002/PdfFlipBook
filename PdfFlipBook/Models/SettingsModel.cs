using PdfFlipBook.Annotations;

namespace PdfFlipBook.Models
{
    public class SettingsModel
    {
        [CanBeNull] public string Password { get; set; }
        [CanBeNull] public int InactivityTime { get; set; }
        [CanBeNull] public int IntervalSwitchPage { get; set; }
        [CanBeNull] public bool RepeatOrNextPage { get; set; }
    }
}
