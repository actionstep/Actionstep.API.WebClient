using System.Collections.Generic;
using System.Linq;

namespace Actionstep.API.WebClient.Paging
{
    public abstract class PageBase<T> where T : class
    {
        public List<T> DataCollection { get; set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
        public int TotalRowCount { get; private set; }
        public int TotalPageCount { get; private set; }


        public PageBase() 
        {
            DataCollection = new List<T>();
        }


        public PageBase(int pageNumber, int pageSize, int totalPageCount, int totalRowCount, IEnumerable<T> source)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalPageCount = totalPageCount;
            TotalRowCount = totalRowCount;

            DataCollection = new List<T>(source.Count());
            DataCollection.AddRange(source);
        }


        public bool HasPreviousPage
        {
            get { return (PageNumber > 1); }
        }


        public bool HasNextPage
        {
            get { return (PageNumber < TotalPageCount); }
        }
    }
}