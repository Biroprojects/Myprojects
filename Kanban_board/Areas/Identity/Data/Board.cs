using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Kanban_board.Areas.Identity.Data
{
    public class Board
    {
        public int BoardId { get; set; }
        public string? Title { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string? CreatedBy { get; set; } // UserId külső kulcsként

        public virtual IdentityUser? CreatedByUser { get; set; }
        public virtual ICollection<BoardUser>? BoardUsers { get; set; }
        public virtual ICollection<List>? Lists { get; set; }
    }
}
