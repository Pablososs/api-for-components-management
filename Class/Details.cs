namespace DAPI.Class
{
    public class Detail
    {
        public int ID { get; set; }

        public string? Parameter { get; set; }

        public string? Description { get; set; }

        public float Value { get; set; }

        public DateTime DateEntry { get; set; }

        public int FK { get; set; }
        public string? Note { get; set; }

        public float GreenLimit { get; set; }

        public float YellowLimit { get; set; }

    }
}
