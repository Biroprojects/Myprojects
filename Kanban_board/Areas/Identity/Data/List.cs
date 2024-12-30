using System.ComponentModel.DataAnnotations;

namespace Kanban_board.Areas.Identity.Data
{
    public class List
    {
        [Key]
        public int ListId { get; set; }
        public int BoardId { get; set; }
        public string Title { get; set; }
        public int Position { get; set; } // Sorrend

        public virtual Board? Board { get; set; }
        public virtual ICollection<KarbanTask>? Tasks { get; set; }
    }
}
