using Kanban_board.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Kanban_board.Pages.Tasks
{
    public class UpdateTaskListModel : PageModel
    {
        private readonly Kanban_boardContext _context;

        public UpdateTaskListModel(Kanban_boardContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OnPostAsync([FromBody] UpdateTaskListDto dto)
        {
            var task = await _context.KanbanTasks.FindAsync(dto.TaskId);
            if (task != null)
            {
                var oldListId = task.ListId; // Eredeti lista ID-ja
                var newListId = dto.NewListId;

                var tasksInNewList = await _context.KanbanTasks
          .Where(t => t.ListId == newListId)
          .ToListAsync();

                bool isNewListEmptyBefore = !tasksInNewList.Any();
                // Friss�tj�k a task list�j�t
                task.ListId = dto.NewListId;
                await _context.SaveChangesAsync();

                // Ellen�rizz�k, hogy az eredeti lista �res lett-e
                var tasksInOldList = await _context.KanbanTasks
                    .Where(t => t.ListId == oldListId)
                    .ToListAsync();


                bool isOldListEmpty = !tasksInOldList.Any();  // Ha nincs t�bb task az eredeti list�ban
               
                // Visszat�r�nk a sikeres v�lasszal, �s jelezz�k, hogy az eredeti lista �res-e
                return new JsonResult(new { success = true, isOldListEmpty, isNewListEmptyBefore });
            }

            return new JsonResult(new { success = false });
        }

        public class UpdateTaskListDto
        {
            public int TaskId { get; set; }
            public int NewListId { get; set; }
        }
    }
}
