using Kanban_board.Areas.Identity.Data;
using Kanban_board.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kanban_board.Pages.Boards
{
    public class AddUserModel : PageModel
    {
        private readonly Kanban_boardContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AddUserModel(Kanban_boardContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public int BoardId { get; set; }

        [BindProperty]
        public string SelectedUserId { get; set; }

        public List<IdentityUser> Users { get; set; } = new List<IdentityUser>();

        public async Task<IActionResult> OnGetAsync(int boardId)
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            BoardId = boardId;
            var AllUsers = await _userManager.Users.ToListAsync();

            var existingUserIds = await _context.BoardUsers
        .Where(bu => bu.BoardId == BoardId)
       .Select(bu => bu.UserId)
       .ToListAsync();

            foreach (var user in AllUsers)
            {
                var roles=await _userManager.GetRolesAsync(user);
                if (!roles.Contains("Admin") && !existingUserIds.Contains(user.Id)) // Szûrés
                {
                    Users.Add(user);
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!User.IsInRole("Admin"))
            {
                return Forbid();
            }

            if (SelectedUserId == null || BoardId == 0)
            {
                return Page();
            }

            // Ellenõrizzük, hogy a felhasználó már hozzá van-e adva a boardhoz
            var existingEntry = await _context.BoardUsers
                .FirstOrDefaultAsync(bu => bu.BoardId == BoardId && bu.UserId == SelectedUserId);

            if (existingEntry == null)
            {
                var boardUser = new BoardUser
                {
                    BoardId = BoardId,
                    UserId = SelectedUserId
                };

                _context.BoardUsers.Add(boardUser);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("/Boards/Index");
        }
    }
}
