/*
week7 prompt:
"
"You are given an existing index.cshtml.cs backend file for a Razor Pages application that manages a list of classes.

Update the IndexModel class to support JSON export functionality as follows:
Requirements:

    Do not modify the frontend (index.cshtml). Only work inside the index.cshtml.cs (PageModel) file.

    Add a new POST handler method named OnPostExportJson with the following signature:

public IActionResult OnPostExportJson(bool filtered, List<string> selectedColumns, string? filter)

    In this method:

    If filtered is true, apply the same filtering logic as in the OnGet() method using the provided filter string.

    If filtered is false, use the full class list (ClassList) without any filtering.

    Project the data into a ClassInformationTable list.

    Use a Utils singleton helper to convert the list to JSON. This helper should be implemented separately.

    Return the resulting JSON as a downloadable file named export.json.

    Make sure the Utils class is imported (e.g. using ClassManApp.Helpers;).

    Do not modify anything else in the existing page model (e.g., add/edit/delete logic should stay untouched).

Your goal is to enable exporting either all or only filtered class records in JSON format based on user input from the frontend."
Helpers/Utils.cs:"week7 utils.cs content will be located here"
Pages/Index.cshtml:"week7 Index.cshtml content will be located here"
"
*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using ClassManApp.Helpers;
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
        public IActionResult OnPostExportJson(bool filtered, List<string> selectedColumns, string? filter)
        {
            var baseData = filtered
                ? ClassList.Where(c => string.IsNullOrWhiteSpace(filter) || c.ClassName.Contains(filter, StringComparison.OrdinalIgnoreCase)).ToList()
                : ClassList;

            var exportData = baseData.Select(c => new ClassInformationTable
            {
                Id = c.Id,
                ClassName = c.ClassName,
                StudentCount = c.StudentCount,
                Description = c.Description
            }).ToList();

            var json = Utils.Instance.ToJson(exportData, selectedColumns);
            var bytes = System.Text.Encoding.UTF8.GetBytes(json);
            return File(bytes, "application/json", "export.json");
        }
    }
}