namespace OTracking.Models.Comun
{
    public class TreeView
    {
        public string text { get; set; }
        public TreeView[] nodes { get; set; }
        public TreeViewState state { get; set; }
        public bool selectable { get; set; }
        public int MenuId { get; set; }
    }

    public class TreeViewState
    {
        public bool disabled { get; set; }
        public bool expanded { get; set; }
        public bool selected { get; set; }
        public bool @checked { get; set; }
    }
}