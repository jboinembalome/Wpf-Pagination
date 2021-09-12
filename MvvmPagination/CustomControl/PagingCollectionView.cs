using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Data;

namespace MvvmPagination.CustomControl
{
    public class PagingCollectionView<T> : CollectionView where T : class
    {
        private readonly ICollection<T> _collection;

        public PagingCollectionView(ICollection<T> collection, int itemsPerPage) : base(collection)
        {
            _collection = collection;
            ItemsPerPage = itemsPerPage;
        }

        public override int Count => ItemsPerPage;

        private int currentPage = 1;
        public int CurrentPage
        {
            get => currentPage;
            set
            {
                currentPage = value;
                OnPropertyChanged(new PropertyChangedEventArgs("CurrentPage"));
                OnPropertyChanged(new PropertyChangedEventArgs("FirstItemNumber"));
                OnPropertyChanged(new PropertyChangedEventArgs("LastItemNumber"));
            }
        }

        public int ItemsPerPage { get; }

        public int TotalPages => (_collection.Count() + ItemsPerPage - 1) / ItemsPerPage;

        public int LastItemNumber
        {
            get
            {
                var end = currentPage * ItemsPerPage - 1;
                end = (end > _collection.Count()) ? _collection.Count() : end;

                return end + 1;
            }
        }

        public int CurrentPageIndex => (currentPage - 1) * ItemsPerPage;

        public int FirstItemNumber => ((currentPage - 1) * ItemsPerPage) + 1;

        public override object GetItemAt(int index)
        {
            var offset = index % ItemsPerPage;
            var position = CurrentPageIndex + offset;

            if (position >= _collection.Count)
                position = _collection.Count - 1;

            return _collection.ElementAt(position);
        }

        public void MoveToNextPage()
        {
            if (CurrentPage < TotalPages)
                CurrentPage += 1;

            Refresh();
        }

        public void MoveToPage(int page)
        {
            CurrentPage = page;

            Refresh();
        }

        public void MoveToPreviousPage()
        {
            if (CurrentPage > 1)
                CurrentPage -= 1;
            Refresh();
        }

        public void MoveToFirstPage()
        {
            CurrentPage = 1;
            Refresh();
        }

        public void MoveToLastPage()
        {
            CurrentPage = TotalPages;
            Refresh();
        }

        public IEnumerable<int> CalculatePageList()
        {
            const int MAX_PAGE_RANGE = 5;
            const int PAGE_DELTA = 2;

            int? startPage = 1;
            int maxPage = TotalPages >= MAX_PAGE_RANGE ? MAX_PAGE_RANGE : TotalPages;

            if (TotalPages > MAX_PAGE_RANGE && CurrentPage > PAGE_DELTA)
                startPage = CurrentPage - PAGE_DELTA;

            if (TotalPages > MAX_PAGE_RANGE && CurrentPage > TotalPages - PAGE_DELTA)
                startPage = TotalPages - (MAX_PAGE_RANGE - 1);

            return Enumerable.Range(startPage.Value, maxPage);
        }

    }
}
