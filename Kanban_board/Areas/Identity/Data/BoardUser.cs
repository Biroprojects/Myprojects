using Microsoft.AspNetCore.Identity;

namespace Kanban_board.Areas.Identity.Data
{
    public class BoardUser
    {
        public int Id { get; set; }
        public int BoardId { get; set; }
        public string UserId { get; set; }

        public virtual Board Board { get; set; }
        public virtual IdentityUser User { get; set; }
    }
}
