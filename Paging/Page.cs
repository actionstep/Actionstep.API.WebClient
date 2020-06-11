using System.Collections.Generic;

namespace Actionstep.API.WebClient.Paging
{
    public class Page<T> : PageBase<T> where T : class
    {
        public Page() { }

        public Page(int pageNumber, int pageSize, int totalPageCount, int totalRowCount, IEnumerable<T> source) : base(pageNumber, pageSize, totalPageCount, totalRowCount, source) { }
    }
}