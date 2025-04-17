using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ClassManApp.Models;

namespace ClassManApp.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public string? Filter { get; set; }

        [FromQuery(Name = "page")]
        public int CurrentPage { get; set; } = 1;


        public int PageSize { get; set; } = 10;

        public int TotalPages { get; set; }

        public class ClassInformationTable
        {
            public int Id { get; set; }
            public string ClassName { get; set; } = string.Empty;
            public int StudentCount { get; set; }
            public string Description { get; set; } = string.Empty;
        }

        public List<ClassInformationTable> DisplayedList { get; set; } = new();

        public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        [BindProperty]
        public ClassInputModel Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public int? EditId { get; set; }

        public bool IsEdit => EditId.HasValue;


        public void OnGet()
        {
            if (!ClassList.Any())
            {
                for (int i = 1; i <= 100; i++)
                {
                    ClassList.Add(new ClassInformationModel
                    {
                        ClassName = $"Class {i}",
                        Description = $"Description for class {i}",
                        StudentCount = 10 + (i % 20)
                    });
                }
            }

            var filtered = string.IsNullOrWhiteSpace(Filter)
                ? ClassList
                : ClassList.Where(c => c.ClassName.Contains(Filter, StringComparison.OrdinalIgnoreCase)).ToList();

            int totalItems = filtered.Count;
            TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

            DisplayedList = filtered
                .Skip((CurrentPage - 1) * PageSize)
                .Take(PageSize)
                .Select(c => new ClassInformationTable
                {
                    Id = c.Id,
                    ClassName = c.ClassName,
                    StudentCount = c.StudentCount,
                    Description = c.Description
                })
                .ToList();


            if (IsEdit)
            {
                var item = ClassList.FirstOrDefault(c => c.Id == EditId);
                if (item != null)
                {
                    Input = new ClassInputModel
                    {
                        ClassName = item.ClassName,
                        StudentCount = item.StudentCount,
                        Description = item.Description
                    };
                }
            }
        }

        public IActionResult OnPostAdd()
        {
            if (!ModelState.IsValid)
                return Page();

            var newItem = new ClassInformationModel
            {
                ClassName = Input.ClassName,
                StudentCount = Input.StudentCount,
                Description = Input.Description
            };

            ClassList.Add(newItem);
            return RedirectToPage();
        }

        public IActionResult OnPostEdit()
        {
            if (!ModelState.IsValid || !EditId.HasValue)
                return Page();

            var item = ClassList.FirstOrDefault(c => c.Id == EditId.Value);
            if (item != null)
            {
                item.ClassName = Input.ClassName;
                item.StudentCount = Input.StudentCount;
                item.Description = Input.Description;
            }

            return RedirectToPage();
        }

        public IActionResult OnPostDelete(int id)
        {
            var item = ClassList.FirstOrDefault(c => c.Id == id);
            if (item != null)
            {
                ClassList.Remove(item);
            }

            return RedirectToPage();
        }

        public class ClassInputModel
        {
            [Required]
            public string ClassName { get; set; } = string.Empty;

            [Range(1, 500)]
            public int StudentCount { get; set; }

            [Required]
            public string Description { get; set; } = string.Empty;
        }
    }
}
