using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Kanban_board.Areas.Identity.Data;
using Kanban_board.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;

namespace Kanban_board.Pages.Boards
{
    public class IndexModel : PageModel
    {
        private readonly Kanban_boardContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(Kanban_boardContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public List<Board> Boards { get; set; }
        public HashSet<int> UserBoardIds { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            // Az összes tábla betöltése
            Boards = await _context.Boards.ToListAsync();

            // A bejelentkezett felhasználó által elérhető táblák lekérése
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            UserBoardIds = new HashSet<int>();

            if (!string.IsNullOrEmpty(userId))
            {
                UserBoardIds = (await _context.BoardUsers
                    .Where(bu => bu.UserId == userId)
                    .Select(bu => bu.BoardId)
                    .ToListAsync())
                    .ToHashSet();
            }

            return Page();
        }
    }
}
