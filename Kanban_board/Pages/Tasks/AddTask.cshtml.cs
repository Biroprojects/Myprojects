using Kanban_board.Areas.Identity.Data;
using Kanban_board.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Kanban_board.Pages.Tasks
{
    public class AddTaskModel : PageModel
    {
        private readonly Kanban_boardContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public AddTaskModel(Kanban_boardContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty]
        public KarbanTask Task { get; set; }

        public int ListId { get; set; }
        public int BoardId { get; set; } // BoardId hozzáadása

        public List<string> UserEmails { get; set; } = new List<string>(); // Felhasználók email címei

        public async Task<IActionResult> OnGetAsync(int listId)
        {
           
            Console.WriteLine("Helloka");
            Console.WriteLine(UserEmails.Count);
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login"); 
            }

            ListId = listId;
            var list = await _context.Lists.FindAsync(ListId);
            if (list == null)
            {
                return RedirectToPage("/Boards/Index");
            }

            BoardId = list.BoardId;

            // Csak admin számára elérhetõ felhasználói email lista
            if (User.IsInRole("Admin")) // Ellenõrzés, hogy a felhasználó admin-e
            {
                UserEmails = await _userManager.Users.Select(u => u.Email).ToListAsync();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int listId)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToPage("/Account/Login");
            }

            ListId = listId;
            var list = await _context.Lists.FindAsync(ListId);
            if (list == null)
            {
                return RedirectToPage("/Boards/Index");
            }

            BoardId = list.BoardId;
            Task.ListId = ListId;

            // Csak admin számára elérhetõ felhasználói email lista
            if (User.IsInRole("Admin")) 
            {
                UserEmails = await _userManager.Users.Select(u => u.Email).ToListAsync();
            }

            Task.CreatedBy = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Admin felhasználó esetén az AssignedTo értéke a kiválasztott email
            if (User.IsInRole("Admin"))
            {
                var userEmail = Request.Form["AssignedTo"]; // Az email cím
                if (string.IsNullOrEmpty(userEmail))
                {
                    ModelState.AddModelError("AssignedTo", "No user assigned.");
                    return Page(); 
                }

                var user = await _userManager.FindByEmailAsync(userEmail);
                if (user != null)
                {
                    Task.AssignedTo = user.Id;
                }
                else
                {
                    ModelState.AddModelError("AssignedTo", "User not found.");
                    return Page();
                }
            }
            else
            {
                Task.AssignedTo = User.FindFirstValue(ClaimTypes.NameIdentifier);
            }

            _context.KanbanTasks.Add(Task);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Boards/Details", new { id = BoardId }); // Visszairányítás a Board részleteihez
        }
    }
}
