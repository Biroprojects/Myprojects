using Kanban_board.Areas.Identity.Data;
using Kanban_board.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Kanban_board.Pages.Lists
{
    public class CreateListModel : PageModel
    {
        private readonly Kanban_boardContext _context;

        public CreateListModel(Kanban_boardContext context)
        {
            _context = context;
        }

        [BindProperty]
        public List List { get; set; }

        public Board Board { get; set; }

        public async Task<IActionResult> OnGetAsync(int boardId)
        {
            Board = await _context.Boards.FindAsync(boardId);

            if (Board == null)
            {
                return NotFound();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int boardId)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            List.BoardId = boardId;
            _context.Lists.Add(List);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Boards/Index");
        }
    }
}
