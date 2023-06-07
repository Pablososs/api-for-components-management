namespace DAPI.Class
{
    public class responsiveComponents
    {
        public string? Message { get; set; }
        public List<Component> Components { get; set; }


        // metodo che si chiama come la classe quindi viene eseguito direttamente quando la classe stessa viene inizializzata e esegue il
        // codice all'interno

        public responsiveComponents()
        {
            Components = new List<Component>();
        }
    }
}
