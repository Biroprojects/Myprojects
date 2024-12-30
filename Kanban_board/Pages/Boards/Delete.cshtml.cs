using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Kanban_board.Areas.Identity.Data;
using Kanban_board.Data;

namespace Kanban_board.Pages.Boards
{
    public class DeleteModel : PageModel
    {
        private readonly Kanban_board.Data.Kanban_boardContext _context;

        public DeleteModel(Kanban_board.Data.Kanban_boardContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Board Board { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Board = await _context.Boards.FindAsync(id);

            if (Board == null)
            {
                return NotFound();
            }

            return Page();
        }
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Board = await _context.Boards.FindAsync(id);

            if (Board != null)
            {
                _context.Boards.Remove(Board);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
