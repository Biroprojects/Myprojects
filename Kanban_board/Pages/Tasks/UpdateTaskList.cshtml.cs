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
                // Frissítjük a task listáját
                task.ListId = dto.NewListId;
                await _context.SaveChangesAsync();

                // Ellenõrizzük, hogy az eredeti lista üres lett-e
                var tasksInOldList = await _context.KanbanTasks
                    .Where(t => t.ListId == oldListId)
                    .ToListAsync();


                bool isOldListEmpty = !tasksInOldList.Any();  // Ha nincs több task az eredeti listában
               
                // Visszatérünk a sikeres válasszal, és jelezzük, hogy az eredeti lista üres-e
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
