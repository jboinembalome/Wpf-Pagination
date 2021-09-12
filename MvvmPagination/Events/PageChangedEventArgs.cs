using System.Windows;

namespace MvvmPagination.Events
{
    public class PageChangedEventArgs : RoutedEventArgs
    {
        public PageChangedEventArgs(RoutedEvent EventToRaise, int OldPage, int NewPage, int TotalPages)
            : base(EventToRaise)
        {
            this.OldPage = OldPage;
            this.NewPage = NewPage;
            this.TotalPages = TotalPages;
        }

        #region Properties
        public int OldPage { get; }

        public int NewPage { get; }

        public int TotalPages { get; }
        #endregion
    }
}
