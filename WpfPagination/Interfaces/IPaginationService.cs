using System.Collections.Generic;

namespace WpfPagination.Interfaces
{
    public interface IPaginationService
    {
        IEnumerable<int> CalculatePageList(int totalPages, int currentPage);
        IEnumerable<T> GetItems<T>(ICollection<T> collection, int v, int itemPerPage) where T : class;
    }
}
