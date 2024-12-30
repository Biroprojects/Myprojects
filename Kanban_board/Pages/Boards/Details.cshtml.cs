using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Kanban_board.Areas.Identity.Data;
using Kanban_board.Data;
using Kanban_board.model;
using System.Security.Claims;
using System.Xml.Linq;
using Microsoft.AspNetCore.Identity;
namespace Kanban_board.Pages.Boards
{
    public class DetailsModel : PageModel
    {
        private readonly Kanban_boardContext _context;

        public DetailsModel(Kanban_boardContext context)
        {
            _context = context;
        }

        public Board Board { get; set; }
        public List<Label> AvailableLabels { get; set; }
        public List<TaskDistribution> TaskDistribution { get; set; }
        public List<UserTaskCount> UserTaskCounts { get; set; }
        public List<UserTaskCount2> UserTaskCounts2 { get; set; }
        private readonly UserManager<IdentityUser> _userManager;

        public async Task OnGetAsync(int id)
        {
            await LoadBoardAsync(id);
            await LoadUserTaskCountsAsync(id);

            // Betöltjük a címkéket
            AvailableLabels = await _context.Labels.ToListAsync();

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            UserTaskCounts2 = await _context.KanbanTasks
    .Where(t => t.AssignedTo == userId && t.List.BoardId == id && t.TaskLabel != null) 
    .GroupBy(t => t.TaskLabel.LabelId)
    .Select(g => new UserTaskCount2
    {
        LabelId = g.Key,
        TaskCount = g.Count()
    })
    .ToListAsync();
        }

        [HttpGet]
        public async Task<IActionResult> ExportToXml(int boardId)
        {
            var board = await _context.Boards
                .Include(b => b.Lists)
                    .ThenInclude(l => l.Tasks)
                        .ThenInclude(t => t.AssignedToUser) 
                .FirstOrDefaultAsync(b => b.BoardId == boardId);

            if (board == null)
            {
                return NotFound();
            }

            // XML dokumentum létrehozása
            var xml = new XElement("Board",
                new XAttribute("Id", board.BoardId),
                new XAttribute("Title", board.Title),
                new XElement("Lists",
                    board.Lists.Select(l =>
                        new XElement("List",
                            new XAttribute("Id", l.ListId),
                            new XAttribute("Name", l.Title),
                            new XElement("Tasks",
                                l.Tasks.Select(t =>
                                    new XElement("Task",
                                        new XAttribute("Id", t.TaskId),
                                        new XAttribute("Title", t.Title),
                                        new XAttribute("Description", t.Description),
                                        new XAttribute("AssignedTo", t.AssignedToUser?.UserName ?? "Unassigned")
                                    )
                                )
                            )
                        )
                    )
                )
            );

            var xmlString = xml.ToString();

            return File(System.Text.Encoding.UTF8.GetBytes(xml.ToString()), "application/xml", "board_export.xml");
        }
        [HttpPost]
        public async Task<IActionResult> OnPostUpdateTaskListAsync(int taskId, int listId)
        {
          
            Console.WriteLine($"Received request to update task {taskId} to list {listId}");
            var taskToUpdate = await _context.KanbanTasks.FindAsync(taskId);
            if (taskToUpdate == null)
            {
                return NotFound();
            }

            taskToUpdate.ListId = listId;

            try
            {
                _context.KanbanTasks.Update(taskToUpdate);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                return BadRequest("Error updating task list. Please try again.");
            }

            return new OkResult();
        }
        [HttpGet]
        public async Task<IActionResult> OnPostExportToXmlAsync(int boardId)
        {
            return await ExportToXml(boardId);
        }
        private async Task LoadBoardAsync(int id)
        {
            Board = await _context.Boards
                .Include(b => b.Lists)
                    .ThenInclude(l => l.Tasks)
                        .ThenInclude(t => t.TaskLabel)
                            .ThenInclude(tl => tl.Label)
                .FirstOrDefaultAsync(m => m.BoardId == id);

            AvailableLabels = await _context.Labels.ToListAsync();
        }


        private async Task LoadTaskDistributionAsync(int boardId)
        {
            TaskDistribution = await _context.KanbanTasks
                .Include(t => t.CreatedByUser)
                .Include(t => t.AssignedToUser)
                .Where(t => t.List.BoardId == boardId)
                .GroupBy(t => t.AssignedToUser.UserName)
                .Select(g => new TaskDistribution
                {
                    Status = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        private async Task LoadUserTaskCountsAsync(int boardId)
        {
            UserTaskCounts = await _context.KanbanTasks
                .Where(t => t.List.BoardId == boardId)
                .GroupBy(t => t.AssignedTo) 
                .Select(g => new UserTaskCount
                {
                    UserName = g.FirstOrDefault().AssignedToUser.UserName,
                    TaskCount = g.Count()
                })
                .ToListAsync();
        }

        public async Task<IActionResult> OnPostAddLabelAsync(int taskId, int labelId, int boardId)
        {
            if (boardId <= 0)
            {
                return BadRequest("Board ID is required.");
            }

            await LoadBoardAsync(boardId);
            var taskToUpdate = await _context.KanbanTasks.FindAsync(taskId);
            if (taskToUpdate == null)
            {
                return NotFound();
            }

            var label = await _context.Labels.FindAsync(labelId);
            if (label == null)
            {
                ModelState.AddModelError("", "Selected label not found.");
                return Page();
            }

            await AssignLabelToTask(taskToUpdate, labelId);

            return RedirectToPage(new { id = Board.BoardId });
        }

        private async Task AssignLabelToTask(KarbanTask task, int labelId)
        {
            task.TaskLabel = new TaskLabel
            {
                TaskId = task.TaskId,
                LabelId = labelId
            };

            _context.KanbanTasks.Update(task);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Error while adding the label. Please try again.");
            }
        }

        public async Task<IActionResult> OnPostDeleteTaskAsync(int id, int taskId)
        {
            await LoadBoardAsync(id);

            var taskToDelete = await _context.KanbanTasks.FindAsync(taskId);
            if (taskToDelete == null)
            {
                return NotFound();
            }

            _context.KanbanTasks.Remove(taskToDelete);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Error while deleting the task. Please try again.");
            }

            return RedirectToPage(new { id = Board.BoardId });
        }
    }
    public class UpdateTaskListRequest
    {
        public int TaskId { get; set; }
        public int ListId { get; set; }
    }
}
