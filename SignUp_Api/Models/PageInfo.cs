namespace SignUp_Api.Models
{
    public class PageInfo
    {
        public string StartCursor { get; set; }
        public string EndCursor { get; set; }
        public bool HasNextPage { get; set; }

        public PageInfo(long startCursor, long endCursor, bool hasNextPage)
        {
            StartCursor = StartCursor;
            EndCursor = EndCursor;
            HasNextPage = HasNextPage;
        }
    }
}
