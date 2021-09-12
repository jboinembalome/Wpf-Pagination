using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.CommandWpf;
using MvvmPagination.Interfaces;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace MvvmPagination.ViewModel
{
    public class PaginationViewModel<T> : ViewModelBase where T : class
    {
        private readonly IPaginationService _paginationService;
        private readonly ICollection<T> _collection;
        private readonly int _itemCount;

        public PaginationViewModel(IPaginationService paginationService, ICollection<T> collection)
        {
            _paginationService = paginationService;
            _collection = collection;
            _itemCount = collection.Count;

            ItemPerPage = PageSizes[0];
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

                RaisePropertyChanged(nameof(TotalPages));

                CurrentPageIndex = (CurrentPage > TotalPages) ? TotalPages - 1 : (CurrentPage < 1) ? 0 : CurrentPage - 1;
            }
        }


        public int TotalPages => _itemCount % ItemPerPage == 0 ? _itemCount / ItemPerPage : (_itemCount / ItemPerPage) + 1;

        private int _currentPageIndex;
        public int CurrentPageIndex
        {
            get => _currentPageIndex;
            private set
            {
                _ = Set(() => CurrentPageIndex, ref _currentPageIndex, value);

                RaisePropertyChanged(nameof(CurrentPage));
                RaisePropertyChanged(nameof(PageList));

                UpdateItemsPaginated();
            }
        }

        public int CurrentPage => _itemCount == 0 ? 0 : _currentPageIndex + 1;

        public IEnumerable<int> PageList => _paginationService.CalculatePageList(TotalPages, CurrentPage);

        #endregion

        #region Commands
        #region PreviousPageCommand
        private RelayCommand _previousPageCommand;

        public RelayCommand PreviousPageCommand => _previousPageCommand
                    ?? (_previousPageCommand = new RelayCommand(
                                          () => PreviousPage(),
                                          () => CanExecutePreviousPage()));

        private void PreviousPage() => CurrentPageIndex--;

        private bool CanExecutePreviousPage() => CurrentPageIndex != 0;
        #endregion

        #region NextPageCommand
        private RelayCommand _nextPageCommand;

        public RelayCommand NextPageCommand => _nextPageCommand
                    ?? (_nextPageCommand = new RelayCommand(
                                          () => NextPage(),
                                          () => CanExecuteNextPage()));

        private void NextPage() => CurrentPageIndex++;

        private bool CanExecuteNextPage() => TotalPages - 1 > CurrentPageIndex;

        #endregion

        #region FirstPageCommand
        private RelayCommand _firstPageCommand;

        public RelayCommand FirstPageCommand => _firstPageCommand
                    ?? (_firstPageCommand = new RelayCommand(
                                          () => FirstPage(),
                                          () => CanExecuteFirstPage()));

        private void FirstPage() => CurrentPageIndex = 0;

        private bool CanExecuteFirstPage() => CurrentPageIndex != 0;
        #endregion

        #region LastPageCommand
        private RelayCommand _lastPageCommand;

        public RelayCommand LastPageCommand => _lastPageCommand
                    ?? (_lastPageCommand = new RelayCommand(
                                          () => LastPage(),
                                          () => CanExecuteLastPage()));

        private void LastPage() => CurrentPageIndex = TotalPages - 1;

        private bool CanExecuteLastPage() => CurrentPage != TotalPages;
        #endregion

        #region UpdatePageCommand
        private RelayCommand<int> _updatePageCommand;

        public RelayCommand<int> UpdatePageCommand => _updatePageCommand
                    ?? (_updatePageCommand = new RelayCommand<int>(
                                          param => UpdatePage(param),
                                          param => CanUpdatePage(param)));

        private void UpdatePage(int selectedPage) => CurrentPageIndex = selectedPage - 1;

        private bool CanUpdatePage(int selectedPage) => selectedPage != CurrentPage;
        #endregion

        #endregion

        #region Methods
        private void UpdateItemsPaginated()
        {
            IEnumerable<T> items = _paginationService.GetItems(_collection, CurrentPageIndex * ItemPerPage, ItemPerPage);
            ItemsPaginated = new ObservableCollection<T>(items);
        }
        #endregion
    }
}