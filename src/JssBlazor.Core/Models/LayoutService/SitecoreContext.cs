namespace JssBlazor.Core.Models.LayoutService
{
    public class SitecoreContext
    {
        public bool PageEditing { get; set; }
        public Site Site { get; set; }
        public string PageState { get; set; }
        public string Language { get; set; }
        public long? VisitorIdentificationTimestamp { get; set; }
    }
}
