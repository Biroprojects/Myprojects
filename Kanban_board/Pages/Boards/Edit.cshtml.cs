using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Kanban_board.Areas.Identity.Data;
using Kanban_board.Data;

namespace Kanban_board.Pages.Boards
{
    public class EditModel : PageModel
    {
        private readonly Kanban_board.Data.Kanban_boardContext _context;

        public EditModel(Kanban_board.Data.Kanban_boardContext context)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Csak a Title mezőt módosítjuk
            var existingBoard = await _context.Boards.FindAsync(Board.BoardId);
            if (existingBoard != null)
            {
                existingBoard.Title = Board.Title; // Csak a Title változik
            }

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BoardExists(Board.BoardId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool BoardExists(int id)
        {
            return _context.Boards.Any(e => e.BoardId == id);
        }
    }
}