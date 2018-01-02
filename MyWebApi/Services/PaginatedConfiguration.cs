namespace MyWebApi.Services
{
    public class PaginatedConfiguration
    {
        public PaginatedConfiguration(int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            Enabled = true;
        }

        public PaginatedConfiguration()
        {
            PageIndex = 1;
            PageSize = 10;
            Enabled = true;
        }

        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public bool Enabled { get; set; }
    }
}