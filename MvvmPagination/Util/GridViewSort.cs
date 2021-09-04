﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Documents;
using System.Windows.Controls.Primitives;

namespace MvvmPagination.Util
{
    public class GridViewSort
    {
        #region Public attached properties

        public static ICommand GetCommand(DependencyObject obj) => (ICommand)obj.GetValue(CommandProperty);

        public static void SetCommand(DependencyObject obj, ICommand value) => obj.SetValue(CommandProperty, value);

        // Using a DependencyProperty as the backing store for Command.
        // This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(GridViewSort),
                new UIPropertyMetadata(null,
                    (o, e) =>
                    {
                        if (o is ItemsControl listView && !GetAutoSort(listView))
                        {
                            if (e.OldValue != null && e.NewValue == null)
                                listView.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));

                            if (e.OldValue == null && e.NewValue != null)
                                listView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                        }
                    }
                )
            );

        public static bool GetAutoSort(DependencyObject obj) => (bool)obj.GetValue(AutoSortProperty);

        public static void SetAutoSort(DependencyObject obj, bool value) => obj.SetValue(AutoSortProperty, value);

        // Using a DependencyProperty as the backing store for AutoSort.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AutoSortProperty =
            DependencyProperty.RegisterAttached("AutoSort", typeof(bool), typeof(GridViewSort),
                new UIPropertyMetadata(false,
                    (o, e) =>
                    {
                        if (o is ListView listView && GetCommand(listView) == null)
                        {
                            bool oldValue = (bool)e.OldValue;
                            bool newValue = (bool)e.NewValue;

                            if (oldValue && !newValue)
                                listView.RemoveHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));

                            if (!oldValue && newValue)
                                listView.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(ColumnHeader_Click));
                        }
                    }
                )
            );

        public static string GetPropertyName(DependencyObject obj) => (string)obj.GetValue(PropertyNameProperty);

        public static void SetPropertyName(DependencyObject obj, string value) =>
            obj.SetValue(PropertyNameProperty, value);

        // Using a DependencyProperty as the backing store for PropertyName.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PropertyNameProperty =
            DependencyProperty.RegisterAttached("PropertyName", typeof(string), typeof(GridViewSort),
                new UIPropertyMetadata(null));

        public static bool GetShowSortGlyph(DependencyObject obj) => 
            (bool)obj.GetValue(ShowSortGlyphProperty);

        public static void SetShowSortGlyph(DependencyObject obj, bool value) => 
            obj.SetValue(ShowSortGlyphProperty, value);

        // Using a DependencyProperty as the backing store for ShowSortGlyph.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowSortGlyphProperty =
            DependencyProperty.RegisterAttached("ShowSortGlyph", typeof(bool), typeof(GridViewSort),
                new UIPropertyMetadata(true));

        public static ImageSource GetSortGlyphAscending(DependencyObject obj) =>
            (ImageSource)obj.GetValue(SortGlyphAscendingProperty);

        public static void SetSortGlyphAscending(DependencyObject obj, ImageSource value) =>
            obj.SetValue(SortGlyphAscendingProperty, value);

        // Using a DependencyProperty as the backing store for SortGlyphAscending.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortGlyphAscendingProperty =
            DependencyProperty.RegisterAttached("SortGlyphAscending", typeof(ImageSource), typeof(GridViewSort),
                new UIPropertyMetadata(null));

        public static ImageSource GetSortGlyphDescending(DependencyObject obj) =>
            (ImageSource)obj.GetValue(SortGlyphDescendingProperty);

        public static void SetSortGlyphDescending(DependencyObject obj, ImageSource value) => 
            obj.SetValue(SortGlyphDescendingProperty, value);

        // Using a DependencyProperty as the backing store for SortGlyphDescending.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SortGlyphDescendingProperty =
            DependencyProperty.RegisterAttached("SortGlyphDescending", typeof(ImageSource), typeof(GridViewSort), 
                new UIPropertyMetadata(null));

        #endregion

        #region Private attached properties
        private static GridViewColumnHeader GetSortedColumnHeader(DependencyObject obj) => 
            (GridViewColumnHeader)obj.GetValue(SortedColumnHeaderProperty);

        private static void SetSortedColumnHeader(DependencyObject obj, GridViewColumnHeader value) => 
            obj.SetValue(SortedColumnHeaderProperty, value);

        // Using a DependencyProperty as the backing store for SortedColumn.  This enables animation, styling, binding, etc...
        private static readonly DependencyProperty SortedColumnHeaderProperty =
            DependencyProperty.RegisterAttached("SortedColumnHeader", typeof(GridViewColumnHeader), typeof(GridViewSort), 
                new UIPropertyMetadata(null));

        #endregion

        #region Column header click event handler

        private static void ColumnHeader_Click(object sender, RoutedEventArgs e)
        {
            if (e.OriginalSource is GridViewColumnHeader headerClicked && headerClicked.Column != null)
            {
                string propertyName = GetPropertyName(headerClicked.Column);
                if (!string.IsNullOrEmpty(propertyName))
                {
                    ListView listView = GetAncestor<ListView>(headerClicked);
                    if (listView != null)
                    {
                        ICommand command = GetCommand(listView);
                        if (command != null)
                        {
                            if (command.CanExecute(propertyName))
                                command.Execute(propertyName);
                            else if (GetAutoSort(listView))
                                ApplySort(listView.Items, propertyName, listView, headerClicked);
                        }
                    }
                }
            }
        }

        #endregion

        #region Helper methods

        public static T GetAncestor<T>(DependencyObject reference) where T : DependencyObject
        {
            DependencyObject parent = VisualTreeHelper.GetParent(reference);
            while (!(parent is T))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
                return (T)parent;
            else
                return null;
        }

        public static void ApplySort(ICollectionView view, string propertyName, ListView listView, 
            GridViewColumnHeader sortedColumnHeader)
        {
            ListSortDirection direction = ListSortDirection.Ascending;
            if (view.SortDescriptions.Count > 0)
            {
                SortDescription currentSort = view.SortDescriptions[0];
                if (currentSort.PropertyName == propertyName)
                {
                    if (currentSort.Direction == ListSortDirection.Ascending)
                        direction = ListSortDirection.Descending;
                    else
                        direction = ListSortDirection.Ascending;
                }

                view.SortDescriptions.Clear();

                GridViewColumnHeader currentSortedColumnHeader = GetSortedColumnHeader(listView);
                if (currentSortedColumnHeader != null)
                    RemoveSortGlyph(currentSortedColumnHeader);
            }
            if (!string.IsNullOrEmpty(propertyName))
            {
                view.SortDescriptions.Add(new SortDescription(propertyName, direction));
                if (GetShowSortGlyph(listView))
                    AddSortGlyph(sortedColumnHeader, direction, direction == ListSortDirection.Ascending 
                        ? GetSortGlyphAscending(listView) 
                        : GetSortGlyphDescending(listView));

                SetSortedColumnHeader(listView, sortedColumnHeader);
            }
        }

        private static void AddSortGlyph(GridViewColumnHeader columnHeader, ListSortDirection direction, 
            ImageSource sortGlyph)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            adornerLayer.Add(new SortGlyphAdorner(columnHeader, direction, sortGlyph));
        }

        private static void RemoveSortGlyph(GridViewColumnHeader columnHeader)
        {
            AdornerLayer adornerLayer = AdornerLayer.GetAdornerLayer(columnHeader);
            Adorner[] adorners = adornerLayer.GetAdorners(columnHeader);

            if (adorners != null)
            {
                foreach (Adorner adorner in adorners)
                {
                    if (adorner is SortGlyphAdorner)
                        adornerLayer.Remove(adorner);
                }
            }
        }

        #endregion

        #region SortGlyphAdorner nested class

        private class SortGlyphAdorner : Adorner
        {
            private readonly GridViewColumnHeader _columnHeader;
            private readonly ListSortDirection _direction;
            private readonly ImageSource _sortGlyph;

            public SortGlyphAdorner(GridViewColumnHeader columnHeader, ListSortDirection direction, ImageSource sortGlyph)
                : base(columnHeader)
            {
                _columnHeader = columnHeader;
                _direction = direction;
                _sortGlyph = sortGlyph;
            }

            private Geometry GetDefaultGlyph()
            {
                double x1 = _columnHeader.ActualWidth - 13;
                double x2 = x1 + 10;
                double x3 = x1 + 5;
                double y1 = _columnHeader.ActualHeight / 2 - 3;
                double y2 = y1 + 5;

                if (_direction == ListSortDirection.Ascending)
                {
                    double tmp = y1;
                    y1 = y2;
                    y2 = tmp;
                }

                PathSegmentCollection pathSegmentCollection = new PathSegmentCollection
                {
                    new LineSegment(new Point(x2, y1), true),
                    new LineSegment(new Point(x3, y2), true)
                };

                PathFigure pathFigure = new PathFigure(new Point(x1, y1), pathSegmentCollection, true);

                PathFigureCollection pathFigureCollection = new PathFigureCollection
                {
                    pathFigure
                };

                PathGeometry pathGeometry = new PathGeometry(pathFigureCollection);
                return pathGeometry;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                if (_sortGlyph != null)
                {
                    double x = _columnHeader.ActualWidth - 13;
                    double y = _columnHeader.ActualHeight / 2 - 5;
                    Rect rect = new Rect(x, y, 10, 10);
                    drawingContext.DrawImage(_sortGlyph, rect);
                }
                else
                    drawingContext.DrawGeometry(Brushes.LightGray, new Pen(Brushes.Gray, 1.0), GetDefaultGlyph());
            }
        }

        #endregion
    }
}
