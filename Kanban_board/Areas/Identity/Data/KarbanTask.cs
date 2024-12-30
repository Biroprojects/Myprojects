using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kanban_board.Areas.Identity.Data
{
    public class KarbanTask
    {
        [Key]
        public int TaskId { get; set; }
        public int ListId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        [ForeignKey("CreatedByUser")]
        public string CreatedBy { get; set; } // UserId külső kulcsként
        [ForeignKey("AssignedToUser")]
        public string? AssignedTo { get; set; } // UserId külső kulcsként

        public int? TaskLabelId { get; set; }
        public virtual List? List { get; set; }
        public virtual IdentityUser? CreatedByUser { get; set; }
        public virtual IdentityUser? AssignedToUser { get; set; }
        public virtual TaskLabel? TaskLabel { get; set; }
    }
}
