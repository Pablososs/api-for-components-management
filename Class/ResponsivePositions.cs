using DAPI.Class;

namespace DAPI.Class
{
    public class responsivePosition
    {


        public Component MyComponent { get; set; }


        public List<Detail> Details { get; set; }


        public Position Position { get; set; }

        public responsivePosition()
        {
            MyComponent = new Component();

            Position = new Position();

            Details = new List<Detail>();
        }





    }
}
