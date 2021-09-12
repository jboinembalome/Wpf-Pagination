using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MvvmPagination.ViewModel
{
    public class PaginationViewModel<T> : ViewModelBase where T : class
    {
        private readonly ICollection<T> _collection;
        private readonly int _itemCount;

        public PaginationViewModel(ICollection<T> collection)
        {
            _collection = collection;
            _itemCount = collection.Count;

            ItemPerPage = PageSizes[0];

            RefreshData();
        }

        #region Properties
        private ObservableCollection<T> _itemsPaginated;
        public ObservableCollection<T> ItemsPaginated
        {
            get => _itemsPaginated;
            set => Set(() => ItemsPaginated, ref _itemsPaginated, value);
        }

        public List<int> PageSizes => new List<int> { 5, 10, 15, 20, 50, 100 };

        private int _itemPerPage;
        public int ItemPerPage
        {
            get => _itemPerPage;
            set
            {
                _ = Set(() => ItemPerPage, ref _itemPerPage, value);

                // Update CurrentPageIndex 
                CurrentPageIndex = (CurrentPage > TotalPages) ? TotalPages - 1 : (CurrentPage < 1) ? 0 : CurrentPage - 1;

                RaisePropertyChanged(nameof(TotalPages));

                RefreshData();
            }
        }


        public int TotalPages =>
            _itemCount % ItemPerPage == 0 ? _itemCount / ItemPerPage : (_itemCount / ItemPerPage) + 1;

        private int _currentPageIndex;
        public int CurrentPageIndex
        {
            get => _currentPageIndex;
            private set
            {
                _ = Set(() => CurrentPageIndex, ref _currentPageIndex, value);

                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(PageList));
            }
        }

        public int CurrentPage => _itemCount == 0 ? 0 : _currentPageIndex + 1;

        public IEnumerable<int> PageList => CalculatePageList();

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
            CurrentPageIndex--;
            RefreshData();
        }

        private bool CanExecutePreviousPage() => CurrentPageIndex != 0;
        #endregion

        #region NextPageCommand
        private RelayCommand _nextPageCommand;

        public RelayCommand NextPageCommand => _nextPageCommand
                    ?? (_nextPageCommand = new RelayCommand(
                                          () => NextPage(),
                                          () => CanExecuteNextPage()));

        private void NextPage()
        {
            CurrentPageIndex++;
            RefreshData();
        }

        private bool CanExecuteNextPage() => TotalPages - 1 > CurrentPageIndex;

        #endregion

        #region FirstPageCommand
        private RelayCommand _firstPageCommand;

        public RelayCommand FirstPageCommand => _firstPageCommand
                    ?? (_firstPageCommand = new RelayCommand(
                                          () => FirstPage(),
                                          () => CanExecuteFirstPage()));

        private void FirstPage()
        {
            CurrentPageIndex = 0;
            RefreshData();
        }

        private bool CanExecuteFirstPage() => CurrentPageIndex != 0;
        #endregion

        #region LastPageCommand
        private RelayCommand _lastPageCommand;

        public RelayCommand LastPageCommand => _lastPageCommand
                    ?? (_lastPageCommand = new RelayCommand(
                                          () => LastPage(),
                                          () => CanExecuteLastPage()));

        private void LastPage()
        {
            CurrentPageIndex = TotalPages - 1;
            RefreshData();
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
            CurrentPageIndex = selectedPage - 1;
            RefreshData();
        }

        private bool CanUpdatePage(int selectedPage) => selectedPage != CurrentPage;
        #endregion

        #endregion

        #region Methods
        private void RefreshData()
        {
            IEnumerable<T> items = _collection.Skip(CurrentPageIndex * ItemPerPage).Take(ItemPerPage);
            ItemsPaginated = new ObservableCollection<T>(items);
        }

        private IEnumerable<int> CalculatePageList()
        {
            const int MAX_PAGE_RANGE = 5;
            const int PAGE_DELTA = 2;

            int startPage = 1;
            int maxPage = TotalPages >= MAX_PAGE_RANGE ? MAX_PAGE_RANGE : TotalPages;

            if (TotalPages > MAX_PAGE_RANGE && CurrentPage > PAGE_DELTA)
                startPage = CurrentPage - PAGE_DELTA;

            if (TotalPages > MAX_PAGE_RANGE && CurrentPage > TotalPages - PAGE_DELTA)
                startPage = TotalPages - (MAX_PAGE_RANGE - 1);

            return Enumerable.Range(startPage, maxPage);
        }
        #endregion
    }
}