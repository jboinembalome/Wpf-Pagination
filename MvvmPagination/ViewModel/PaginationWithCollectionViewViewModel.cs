using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmPagination.CustomControl;
using System.Collections.Generic;

namespace MvvmPagination.ViewModel
{
    public class PaginationWithCollectionViewViewModel<T> : ViewModelBase where T : class
    {
        private readonly ICollection<T> _collection;

        public PaginationWithCollectionViewViewModel(ICollection<T> collection)
        {
            _collection = collection;

            ItemPerPage = PageSizes[0];

            PagingCollection = new PagingCollectionView<T>(_collection, ItemPerPage);
        }

        #region Properties

        private PagingCollectionView<T> _pagingCollection;
        public PagingCollectionView<T> PagingCollection
        {
            get => _pagingCollection;
            set => Set(() => PagingCollection, ref _pagingCollection, value);
        }

        public List<int> PageSizes => new List<int> { 5, 10, 15, 20, 50, 100 };

        private int _itemPerPage;
        public int ItemPerPage
        {
            get => _itemPerPage;
            set
            {
                _ = Set(() => ItemPerPage, ref _itemPerPage, value);

                RaisePropertyChanged(nameof(TotalPages));

                PagingCollection = new PagingCollectionView<T>(_collection, ItemPerPage);

                RaisePropertyChanged(nameof(PageList));
            }
        }

        public int TotalPages => PagingCollection.TotalPages;

        public int? CurrentPage => PagingCollection?.CurrentPage;

        public IEnumerable<int> PageList => PagingCollection.CalculatePageList();

        #endregion

        #region Commands
        #region PreviousPageCommand
        private RelayCommand _previousPageCommand;

        public RelayCommand PreviousPageCommand => _previousPageCommand
                    ?? (_previousPageCommand = new RelayCommand(
                                          () => PreviousPage(),
                                          () => CanExecutePreviousPage()));

        private void PreviousPage()
        {
            PagingCollection.MoveToPreviousPage();
            RaisePropertyChanged(nameof(PageList));
        }

        private bool CanExecutePreviousPage() => CurrentPage != 1;
        #endregion

        #region NextPageCommand
        private RelayCommand _nextPageCommand;

        public RelayCommand NextPageCommand => _nextPageCommand
                    ?? (_nextPageCommand = new RelayCommand(
                                          () => NextPage(),
                                          () => CanExecuteNextPage()));

        private void NextPage()
        {
            PagingCollection.MoveToNextPage();
            RaisePropertyChanged(nameof(PageList));
        }

        private bool CanExecuteNextPage() => TotalPages > CurrentPage;

        #endregion

        #region FirstPageCommand
        private RelayCommand _firstPageCommand;

        public RelayCommand FirstPageCommand => _firstPageCommand
                    ?? (_firstPageCommand = new RelayCommand(
                                          () => FirstPage(),
                                          () => CanExecuteFirstPage()));

        private void FirstPage()
        {
            PagingCollection.MoveToFirstPage();
            RaisePropertyChanged(nameof(PageList));
        }

        private bool CanExecuteFirstPage() => CurrentPage != 1;
        #endregion

        #region LastPageCommand
        private RelayCommand _lastPageCommand;

        public RelayCommand LastPageCommand => _lastPageCommand
                    ?? (_lastPageCommand = new RelayCommand(
                                          () => LastPage(),
                                          () => CanExecuteLastPage()));

        private void LastPage()
        {
            PagingCollection.MoveToLastPage();
            RaisePropertyChanged(nameof(PageList));
        }

        private bool CanExecuteLastPage() => CurrentPage != TotalPages;
        #endregion

        #region UpdatePageCommand
        private RelayCommand<int> _updatePageCommand;

        public RelayCommand<int> UpdatePageCommand => _updatePageCommand
                    ?? (_updatePageCommand = new RelayCommand<int>(
                                          param => UpdatePage(param),
                                          param => CanUpdatePage(param)));

        private void UpdatePage(int selectedPage)
        {
            PagingCollection.MoveToPage(selectedPage);
            RaisePropertyChanged(nameof(PageList));
        }

        private bool CanUpdatePage(int selectedPage) => selectedPage != CurrentPage;
        #endregion

        #endregion

    }
}