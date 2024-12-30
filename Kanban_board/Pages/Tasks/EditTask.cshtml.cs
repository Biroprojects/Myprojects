using Kanban_board.Areas.Identity.Data;
using Kanban_board.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kanban_board.Pages.Tasks
{
    public class EditTaskModel : PageModel
    {
        private readonly Kanban_boardContext _context;

        public EditTaskModel(Kanban_boardContext context)
        {
            _context = context;
        }

        [BindProperty]
        public KarbanTask Task { get; set; }

        public async Task<IActionResult> OnGetAsync(int taskId)
        {
  
            // Feladat lekérése az adatbázisból
            Task = await _context.KanbanTasks
                .Include(t => t.List) 
                .FirstOrDefaultAsync(t => t.TaskId == taskId);

            if (Task == null)
            {
                return NotFound();
            }
            return Page();
           
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Console.WriteLine("Ez még lefut");
            Console.WriteLine(Task.TaskId);

            var existingTask = await _context.KanbanTasks
     .Include(t => t.List).FirstOrDefaultAsync(t => t.TaskId == Task.TaskId);

            if (existingTask == null)
            {
                return NotFound();
            }
           
            existingTask.Title = Task.Title;
            existingTask.Description = Task.Description;
            if (existingTask.List == null)
            {

                return NotFound(); 
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Error while updating the task. Please try again.");
                return Page();
            }

            
            return RedirectToPage("/Boards/Details", new { id = existingTask.List.BoardId });
        }
    }
}
