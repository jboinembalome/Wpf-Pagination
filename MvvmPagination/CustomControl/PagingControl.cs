using WpfPagination.Enums;
using WpfPagination.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace WpfPagination.CustomControl
{
    [TemplatePart(Name = "PART_FirstPageButton", Type = typeof(Button)),
     TemplatePart(Name = "PART_PreviousPageButton", Type = typeof(Button)),
     TemplatePart(Name = "PART_FirstButton", Type = typeof(Button)),
     TemplatePart(Name = "PART_SecondButton", Type = typeof(Button)),
     TemplatePart(Name = "PART_ThirdButton", Type = typeof(Button)),
     TemplatePart(Name = "PART_FourthButton", Type = typeof(Button)),
     TemplatePart(Name = "PART_FifthButton", Type = typeof(Button)),
     TemplatePart(Name = "PART_NextPageButton", Type = typeof(Button)),
     TemplatePart(Name = "PART_LastPageButton", Type = typeof(Button)),
     TemplatePart(Name = "PART_PageSizesCombobox", Type = typeof(ComboBox))]
    public class PagingControl : Control
    {
        protected Button btnFirstPage, btnPreviousPage, btnFirst, btnSecond, btnThird, btnFourth, btnFifth, btnNextPage, btnLastPage;
        protected ComboBox cmbPageSizes;

        #region Contructors

        static PagingControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PagingControl), new FrameworkPropertyMetadata(typeof(PagingControl)));

            ItemsPaginatedProperty = DependencyProperty.Register("ItemsPaginated", typeof(ObservableCollection<object>), typeof(PagingControl), new PropertyMetadata(new ObservableCollection<object>()));
            CurrentPageProperty = DependencyProperty.Register("CurrentPage", typeof(int), typeof(PagingControl));
            TotalPagesProperty = DependencyProperty.Register("TotalPages", typeof(int), typeof(PagingControl));
            PageSizesProperty = DependencyProperty.Register("PageSizes", typeof(ObservableCollection<int>), typeof(PagingControl), new PropertyMetadata(new ObservableCollection<int>()));
            ItemsSourceProperty = DependencyProperty.Register("ItemsSource", typeof(ICollection), typeof(PagingControl));

            PreviewPageChangeEvent = EventManager.RegisterRoutedEvent("PreviewPageChange", RoutingStrategy.Bubble, typeof(PageChangedEventHandler), typeof(PagingControl));
            PageChangedEvent = EventManager.RegisterRoutedEvent("PageChanged", RoutingStrategy.Bubble, typeof(PageChangedEventHandler), typeof(PagingControl));
        }

        public PagingControl()
        {
            Loaded += new RoutedEventHandler(PaggingControlLoaded);
        }

        ~PagingControl()
        {
            UnregisterEvents();
        }

        #endregion

        #region Properties
        public static readonly DependencyProperty ItemsPaginatedProperty;
        public static readonly DependencyProperty CurrentPageProperty;
        public static readonly DependencyProperty TotalPagesProperty;
        public static readonly DependencyProperty PageSizesProperty;
        public static readonly DependencyProperty ItemsSourceProperty;

        public ObservableCollection<object> ItemsPaginated
        {
            get => GetValue(ItemsPaginatedProperty) as ObservableCollection<object>;
            private set => SetValue(ItemsPaginatedProperty, value);
        }

        public int CurrentPage
        {
            get => (int)GetValue(CurrentPageProperty);
            set => SetValue(CurrentPageProperty, value);
        }

        public int TotalPages
        {
            get => (int)GetValue(TotalPagesProperty);
            private set => SetValue(TotalPagesProperty, value);
        }

        public ObservableCollection<int> PageSizes => GetValue(PageSizesProperty) as ObservableCollection<int>;

        public ICollection ItemsSource
        {
            get => GetValue(ItemsSourceProperty) as ICollection;
            set => SetValue(ItemsSourceProperty, value);
        }
        #endregion

        #region Events
        public delegate void PageChangedEventHandler(object sender, PageChangedEventArgs args);

        public static readonly RoutedEvent PreviewPageChangeEvent;
        public static readonly RoutedEvent PageChangedEvent;

        public event PageChangedEventHandler PreviewPageChange
        {
            add => AddHandler(PreviewPageChangeEvent, value);
            remove => RemoveHandler(PreviewPageChangeEvent, value);
        }

        public event PageChangedEventHandler PageChanged
        {
            add => AddHandler(PageChangedEvent, value);
            remove => RemoveHandler(PageChangedEvent, value);
        }

        private void PaggingControlLoaded(object sender, RoutedEventArgs e)
        {
            if (Template == null)
                throw new ArgumentNullException("Control template is null.");

            if (ItemsSource == null)
                throw new ArgumentNullException($"{nameof(ItemsSource)} is null.");
        }

        private void BtnFirstPageClick(object sender, RoutedEventArgs e) => Navigate(PageChanges.First);

        private void BtnPreviousPageClick(object sender, RoutedEventArgs e) => Navigate(PageChanges.Previous);

        private void BtnFirstClick(object sender, RoutedEventArgs e) => Navigate(PageChanges.Current, (int)(sender as Button).Content);

        private void BtnSecondClick(object sender, RoutedEventArgs e) => Navigate(PageChanges.Current, (int)(sender as Button).Content);

        private void BtnThirdClick(object sender, RoutedEventArgs e) => Navigate(PageChanges.Current, (int)(sender as Button).Content);

        private void BtnFourthClick(object sender, RoutedEventArgs e) => Navigate(PageChanges.Current, (int)(sender as Button).Content);

        private void BtnFifthClick(object sender, RoutedEventArgs e) => Navigate(PageChanges.Current, (int)(sender as Button).Content);

        private void BtnNextPageClick(object sender, RoutedEventArgs e) => Navigate(PageChanges.Next);

        private void BtnLastPageClick(object sender, RoutedEventArgs e) => Navigate(PageChanges.Last);

        private void CmbPageSizesSelectionChanged(object sender, SelectionChangedEventArgs e) => Navigate(PageChanges.Current);
        #endregion

        #region Methods
        public override void OnApplyTemplate()
        {
            btnFirstPage = Template.FindName("PART_FirstPageButton", this) as Button;
            btnPreviousPage = Template.FindName("PART_PreviousPageButton", this) as Button;
            btnFirst = Template.FindName("PART_FirstButton", this) as Button;
            btnSecond = Template.FindName("PART_SecondButton", this) as Button;
            btnThird = Template.FindName("PART_ThirdButton", this) as Button;
            btnFourth = Template.FindName("PART_FourthButton", this) as Button;
            btnFifth = Template.FindName("PART_FifthButton", this) as Button;
            btnNextPage = Template.FindName("PART_NextPageButton", this) as Button;
            btnLastPage = Template.FindName("PART_LastPageButton", this) as Button;
            cmbPageSizes = Template.FindName("PART_PageSizesCombobox", this) as ComboBox;

            if (btnFirstPage == null || btnPreviousPage == null || btnFirst == null || btnSecond == null || btnThird == null
                || btnFourth == null || btnFifth == null || btnNextPage == null || btnLastPage == null || cmbPageSizes == null)
                throw new Exception("Invalid Control template.");

            RegisterEvents();
            SetDefaultValues();
            BindProperties();

            base.OnApplyTemplate();
        }

        private void RegisterEvents()
        {
            if (btnFirstPage != null && btnPreviousPage != null && btnFirst != null && btnSecond != null && btnThird != null
               && btnFourth != null && btnFifth != null && btnNextPage != null && btnLastPage != null && cmbPageSizes != null)
            {
                btnFirstPage.Click += new RoutedEventHandler(BtnFirstPageClick);
                btnPreviousPage.Click += new RoutedEventHandler(BtnPreviousPageClick);
                btnFirst.Click += new RoutedEventHandler(BtnFirstClick);
                btnSecond.Click += new RoutedEventHandler(BtnSecondClick);
                btnThird.Click += new RoutedEventHandler(BtnThirdClick);
                btnFourth.Click += new RoutedEventHandler(BtnFourthClick);
                btnFifth.Click += new RoutedEventHandler(BtnFifthClick);
                btnNextPage.Click += new RoutedEventHandler(BtnNextPageClick);
                btnLastPage.Click += new RoutedEventHandler(BtnLastPageClick);

                cmbPageSizes.SelectionChanged += new SelectionChangedEventHandler(CmbPageSizesSelectionChanged);
            }
        }

        private void UnregisterEvents()
        {
            if (btnFirstPage != null && btnPreviousPage != null && btnFirst != null && btnSecond != null && btnThird != null
               && btnFourth != null && btnFifth != null && btnNextPage != null && btnLastPage != null && cmbPageSizes != null)
            {
                btnFirstPage.Click -= BtnFirstPageClick;
                btnPreviousPage.Click -= BtnPreviousPageClick;
                btnFirst.Click -= BtnFirstClick;
                btnSecond.Click -= BtnSecondClick;
                btnThird.Click -= BtnThirdClick;
                btnFourth.Click -= BtnFourthClick;
                btnFifth.Click += BtnFifthClick;
                btnNextPage.Click -= BtnNextPageClick;
                btnLastPage.Click -= BtnLastPageClick;

                cmbPageSizes.SelectionChanged -= CmbPageSizesSelectionChanged;
            }
        }

        private void SetDefaultValues()
        {
            ItemsPaginated = new ObservableCollection<object>();

            cmbPageSizes.IsEditable = false;
            cmbPageSizes.SelectedIndex = 0;
        }

        private void BindProperties()
        {
            var propBinding = new Binding("PageSizes")
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.TemplatedParent),
                Mode = BindingMode.TwoWay,
                UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
            };

            _ = cmbPageSizes.SetBinding(ItemsControl.ItemsSourceProperty, propBinding);
        }

        private void RaisePageChanged(int OldPage, int NewPage)
        {
            var args = new PageChangedEventArgs(PageChangedEvent, OldPage, NewPage, TotalPages);
            RaiseEvent(args);
        }

        private void RaisePreviewPageChange(int OldPage, int NewPage)
        {
            var args = new PageChangedEventArgs(PreviewPageChangeEvent, OldPage, NewPage, TotalPages);
            RaiseEvent(args);
        }

        private void Navigate(PageChanges pageChanges, int page = 0)
        {
            if (ItemsSource == null)
                return;

            var itemCount = ItemsSource.Count;
            var itemPerPage = (int)cmbPageSizes.SelectedItem;

            if (itemCount == 0)
            {
                ItemsPaginated.Clear();
                TotalPages = 1;
                CurrentPage = 1;
            }
            else
            {
                TotalPages = (itemCount / itemPerPage) + ((itemCount % itemPerPage == 0) ? 0 : 1);
                CurrentPage = page != 0 ? page : CurrentPage;
            }

            var newPage = CalculateNewPage(pageChanges);
            var currentPageIndex = (newPage - 1) * itemPerPage;
            var oldPage = CurrentPage;

            RaisePreviewPageChange(CurrentPage, newPage);

            CurrentPage = newPage;

            RefreshData(currentPageIndex, itemPerPage);

            RaisePageChanged(oldPage, CurrentPage);

            UpdateButtons();
        }

        /// <summary>
        /// Updates the content and enabling of paging buttons.
        /// </summary>
        private void UpdateButtons()
        {
            SetContentButtonPageList();
            SetEnableButtons();
        }

        private void SetContentButtonPageList()
        {
            var buttonRange = CalculatePageList();

            btnFirst.Content = buttonRange.ElementAt(0);
            btnSecond.Content = buttonRange.ElementAt(1);
            btnThird.Content = buttonRange.ElementAt(2);
            btnFourth.Content = buttonRange.ElementAt(3);
            btnFifth.Content = buttonRange.ElementAt(4);
        }

        private void SetEnableButtons()
        {
            btnFirstPage.IsEnabled = CurrentPage != 1;
            btnPreviousPage.IsEnabled = CurrentPage != 1;

            btnFirst.IsEnabled = (int)btnFirst.Content != CurrentPage;
            btnSecond.IsEnabled = (int)btnSecond.Content != CurrentPage;
            btnThird.IsEnabled = (int)btnThird.Content != CurrentPage;
            btnFourth.IsEnabled = (int)btnFourth.Content != CurrentPage;
            btnFifth.IsEnabled = (int)btnFifth.Content != CurrentPage;

            btnNextPage.IsEnabled = TotalPages > CurrentPage;
            btnLastPage.IsEnabled = CurrentPage != TotalPages;
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

        private int CalculateNewPage(PageChanges pageChanges)
        {
            switch (pageChanges)
            {
                case PageChanges.First:
                    return 1;
                case PageChanges.Previous:
                    return (CurrentPage - 1 > TotalPages) ? TotalPages : (CurrentPage - 1 < 1) ? 1 : CurrentPage - 1;
                case PageChanges.Current:
                    return (CurrentPage > TotalPages) ? TotalPages : (CurrentPage < 1) ? 1 : CurrentPage;
                case PageChanges.Next:
                    return (CurrentPage + 1 > TotalPages) ? TotalPages : CurrentPage + 1;
                case PageChanges.Last:
                    if (CurrentPage == TotalPages)
                        return 1;
                    return TotalPages;
                default:
                    return 1;
            }

        }

        private void RefreshData(int currentPageIndex, int itemPerPage)
        {
            var items = ItemsSource.OfType<object>().Skip(currentPageIndex).Take(itemPerPage);
            ItemsPaginated = new ObservableCollection<object>(items);
        }
        #endregion
    }
}
