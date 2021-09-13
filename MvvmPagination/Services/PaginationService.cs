using WpfPagination.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace WpfPagination.Services
{
    public class PaginationService : IPaginationService
    {
        public IEnumerable<int> CalculatePageList(int totalPages, int currentPage)
        {
            const int MAX_PAGE_RANGE = 5;
            const int PAGE_DELTA = 2;

            var startPage = 1;
            var maxPage = totalPages >= MAX_PAGE_RANGE ? MAX_PAGE_RANGE : totalPages;

            if (totalPages > MAX_PAGE_RANGE && currentPage > PAGE_DELTA)
                startPage = currentPage - PAGE_DELTA;

            if (totalPages > MAX_PAGE_RANGE && currentPage > totalPages - PAGE_DELTA)
                startPage = totalPages - (MAX_PAGE_RANGE - 1);

            return Enumerable.Range(startPage, maxPage);
        }

        public IEnumerable<T> GetItems<T>(ICollection<T> collection, int skip, int take) where T : class 
            => collection.Skip(skip).Take(take);
    }
}
