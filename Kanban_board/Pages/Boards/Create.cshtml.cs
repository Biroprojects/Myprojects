using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Kanban_board.Areas.Identity.Data;
using Kanban_board.Data;
using Microsoft.AspNetCore.Authorization;

namespace Kanban_board.Pages.Boards
{
    [Authorize(Roles = "Admin")]
    public class CreateModel : PageModel
    {
        private readonly Kanban_boardContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger<CreateModel> _logger;

        public CreateModel(Kanban_boardContext context, UserManager<IdentityUser> userManager, ILogger<CreateModel> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public Board Board { get; set; } = new Board();

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Be kell jelentkezned a művelet végrehajtásához.");
                return Page();
            }

            // Állítsuk be a létrehozó nevét a Board objektumban
            Board.CreatedBy = user.UserName;

            // Ha a CreatedBy mező kötelező, a set előtt érdemes ellenőrizni
            if (string.IsNullOrEmpty(Board.CreatedBy))
            {
                ModelState.AddModelError("Board.CreatedBy", "A CreatedBy mező kötelező.");
            }

            if (!ModelState.IsValid)
            {
                // Hibák kilistázása a naplóban
                foreach (var modelStateKey in ModelState.Keys)
                {
                    var value = ModelState[modelStateKey];
                    foreach (var error in value.Errors)
                    {
                        _logger.LogError($"ModelState error in {modelStateKey}: {error.ErrorMessage}");
                    }
                }

                return Page();
            }

            _context.Boards.Add(Board);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}