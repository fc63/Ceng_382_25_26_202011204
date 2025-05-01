using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using ClassManApp.Helpers;
using ClassManApp.Models;
using ClassManApp.Data;

namespace LabProject.Pages
{
    public class ClassManAppModel : PageModel
    {
        private readonly SchoolDbContext _context;

        public ClassManAppModel(SchoolDbContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        [FromQuery(Name = "page")]
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 10;
        public int TotalPages { get; set; }

        public List<ClassInformationModel> DisplayedList { get; set; } = new();

        [BindProperty]
        public ClassInputModel Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? EditId { get; set; }

        public bool IsEdit => EditId.HasValue;

        public async Task<IActionResult> OnGetAsync()
        {
            if (!CheckSessionValidity())
                return RedirectToPage("/Login");

            IQueryable<ClassInformationModel> query = _context.Classes;

            if (!string.IsNullOrWhiteSpace(Filter))
                query = query.Where(c => c.Name.Contains(Filter));

            int totalItems = await query.CountAsync();
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            DisplayedList = await query
                .OrderBy(c => c.Id)
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .ToListAsync();

            if (IsEdit)
            {
                var item = await _context.Classes.FindAsync(EditId);
                if (item != null)
                {
                    Input = new ClassInputModel
                    {
                        Name = item.Name,
                        PersonCount = item.PersonCount,
                        Description = item.Description,
                        IsActive = item.IsActive
                    };
                }
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAddAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            var newItem = new ClassInformationModel
            {
                Name = Input.Name,
                PersonCount = Input.PersonCount,
                Description = Input.Description,
                IsActive = Input.IsActive
            };

            _context.Classes.Add(newItem);
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostEditAsync()
        {
            if (!ModelState.IsValid || !EditId.HasValue)
                return Page();

            var item = await _context.Classes.FindAsync(EditId.Value);
            if (item != null)
            {
                item.Name = Input.Name;
                item.PersonCount = Input.PersonCount;
                item.Description = Input.Description;
                item.IsActive = Input.IsActive;

                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            var item = await _context.Classes.FindAsync(id);
            if (item != null)
            {
                _context.Classes.Remove(item);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostExportJsonAsync(bool filtered, List<string> selectedColumns, string? filter, int currentPage)
        {
            Filter = filter;
            CurrentPage = currentPage;
            await OnGetAsync();

            var json = Utils.Instance.ToJson(DisplayedList, selectedColumns);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", "export.json");
        }

        public IActionResult OnPostLogout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete("username");
            Response.Cookies.Delete("token");
            Response.Cookies.Delete("session_id");
            return RedirectToPage("/Login");
        }

        private bool CheckSessionValidity()
        {
            var tokenFromSession = HttpContext.Session.GetString("token");
            var usernameFromSession = HttpContext.Session.GetString("username");
            var sessionIdFromSession = HttpContext.Session.Id;

            var tokenFromCookie = Request.Cookies["token"];
            var usernameFromCookie = Request.Cookies["username"];
            var sessionIdFromCookie = Request.Cookies["session_id"];

            bool valid = tokenFromSession != null &&
                         usernameFromSession != null &&
                         sessionIdFromSession != null &&
                         tokenFromCookie == tokenFromSession &&
                         usernameFromCookie == usernameFromSession &&
                         sessionIdFromCookie == sessionIdFromSession;

            if (!valid)
            {
                TempData["Error"] = "Unauthorized access.";
                Response.Cookies.Delete("token");
                Response.Cookies.Delete("username");
                Response.Cookies.Delete("session_id");
                HttpContext.Session.Clear();
            }

            return valid;
        }

        public class ClassInputModel
        {
            [Required]
            public string Name { get; set; } = string.Empty;

            [Range(1, 500)]
            public int PersonCount { get; set; }

            [Required]
            public string Description { get; set; } = string.Empty;

            public bool IsActive { get; set; } = true;
        }
    }
}
