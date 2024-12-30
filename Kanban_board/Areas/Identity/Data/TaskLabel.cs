namespace Kanban_board.Areas.Identity.Data
{
    public class TaskLabel
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public int LabelId { get; set; }

        public virtual KarbanTask Task { get; set; }
        public virtual Label Label { get; set; }
    }
}
