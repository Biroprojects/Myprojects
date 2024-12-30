using Kanban_board.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kanban_board.Pages.Lists
{
    public class DeleteListModel : PageModel
    {
        private readonly Kanban_boardContext _context;


        public DeleteListModel(Kanban_boardContext context, ILogger<DeleteListModel> logger)
        {
            _context = context;
        }

        [BindProperty]
        public int ListId { get; set; }
        public string ListTitle { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var list = await _context.Lists
        .Include(l => l.Board) 
        .FirstOrDefaultAsync(l => l.ListId == id);

            if (list == null)
            {
                return NotFound();
            }

            ListId = list.ListId;
            ListTitle = list.Title;

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var list = await _context.Lists.FindAsync(ListId);
            if (list == null)
            {
                return NotFound();
            }

            var boardId = list.BoardId;

            _context.Lists.Remove(list);
            await _context.SaveChangesAsync();
            return RedirectToPage("/Boards/Details", new { id = boardId });
        }
    }
}
