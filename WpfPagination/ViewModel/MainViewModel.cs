using GalaSoft.MvvmLight;
using WpfPagination.Interfaces;
using WpfPagination.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace WpfPagination.ViewModel
{
    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel(IPaginationService paginationService)
        {
            People = new ObservableCollection<Person>(FakePeople());

            PaginationViewModel = new PaginationViewModel<Person>(paginationService, People);

            PaginationWithCollectionViewViewModel = new PaginationWithCollectionViewViewModel<Person>(People);
        }

        #region Properties
        public PaginationViewModel<Person> PaginationViewModel { get; set; }

        public PaginationWithCollectionViewViewModel<Person> PaginationWithCollectionViewViewModel { get; set; }

        private ObservableCollection<Person> _people;
        public ObservableCollection<Person> People
        {
            get => new ObservableCollection<Person>(_people);
            private set => Set(() => People, ref _people, value);
        }
        #endregion

        #region Methods
        private IEnumerable<Person> FakePeople() => Enumerable.Range(1, 1000)
                .Select(i => new Person { Id = i, FirstName = "Jimmy", LastName = "Boinembalome", Birthday = DateTime.Now });
        #endregion
    }
}