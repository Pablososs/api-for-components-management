using DAPI.Class;

namespace DAPI.Class
{
    public class responsiveDetails
    {
        public string? Message { get; set; }

        public List<Detail> Details { get; set; }

        public responsiveDetails()
        {

            Details = new List<Detail>();
        }
    }
}
