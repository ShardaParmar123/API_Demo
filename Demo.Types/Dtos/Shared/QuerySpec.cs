namespace Demo.Types.Dtos.Shared
{
    public class QuerySpec
    {
        public class Pagination
        {
            const int maxPageSize = 50;
            public int PageNumber { get; set; } = 1;
            private int _pageSize = 10;
            public int PageSize
            {
                get
                {
                    return _pageSize;
                }
                set
                {
                    _pageSize = (value > maxPageSize) ? maxPageSize : value;
                }
            }
        }   
        public Pagination? pagination { get; set; }
    }
}
