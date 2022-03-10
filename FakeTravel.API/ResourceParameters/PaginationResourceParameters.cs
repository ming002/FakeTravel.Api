namespace FakeTravel.API.ResourceParameters
{
    public class PaginationResourceParameters
    {

        private int _pageNum = 1;
        public int PageNum
        {
            get
            {
                return _pageNum;
            }
            set
            {
                if (value >= 1)
                {
                    _pageNum = value;
                }
            }
        }

        private int _pageSize = 10;

        const int maxPageSize = 50;
        public int PageSize
        {
            get
            {
                return _pageSize;
            }
            set
            {
                if (value >= 1)
                {
                    _pageSize = (value > maxPageSize) ? maxPageSize : value;
                }
            }
        }
    }
}
