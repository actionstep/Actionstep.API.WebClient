namespace Actionstep.API.WebClient.View_Models
{
    public class FilenoteAddEditViewModel
    {
        public int FilenoteId { get; set; }
        public string ActionId { get; set; }
        public string Content { get; set; }

        public string ColumnSize => FilenoteId == 0 ? "8" : "12";
    }
}