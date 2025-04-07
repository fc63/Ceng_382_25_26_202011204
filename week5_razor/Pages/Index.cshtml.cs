/*
Week 6 Prompt:
"
"Transform the following Index.cshtml.cs file to include filtering and pagination functionality in the backend for a Razor Pages app that manages class data.

Modify and extend the code to satisfy these updated requirements:

✅ Data Structure & Initialization:

    Keep using the static in-memory list ClassList to store all class records.

    Add sample data generation in OnGet() if ClassList is empty (generate at least 100 items with dummy values).

✅ Filtering Support:

    Add a query parameter called Filter (string?, using [BindProperty(SupportsGet = true)]) that is used to filter the class list by ClassName.

    Perform filtering using LINQ inside the OnGet() method.

✅ Pagination Support:

    Add another query parameter page (bound via [FromQuery(Name = "page")]) to control current page.

    Define PageSize (e.g. 10), calculate TotalPages, and use Skip().Take() to paginate the filtered results.

✅ Data Projection for View:

    Create a new inner class ClassInformationTable that holds the fields to be shown in the table: Id, ClassName, StudentCount, and Description.

    Use .Select() to convert filtered data into this display model.

    Assign the result to a new property called DisplayedList.

✅ Form Editing:

    When EditId is provided, pre-fill the Input form with matching data from the main list (same as before).

✅ Preserve:

    Keep form validation and actions: OnPostAdd, OnPostEdit, OnPostDelete just as they are.

    Keep ClassInputModel as-is, unless necessary changes are needed.

✳️ Important: Refactor only where needed. Do not change the logic for form submission or static list unless related to filtering or pagination.

The final result should match a Razor Pages backend that supports:

    Filtering by class name

    Pagination over the filtered result

    Proper binding and display list preparation (DisplayedList)

    Still supports Add, Edit, and Delete actions like before"

Current index.cshtml.cs: "this should contain the week5 index.cshtml.cs code"
week 6 index.cshtml: "this should contain the new week 6 index.cshtml content converted by gpt."

Make sure to update all this in accordance with the contents of the week 6 index.cshtml."

*/

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using week5_razor.Models;

namespace week5_razor.Pages
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
            public string ClassName { get; set; }

            [Range(1, 500)]
            public int StudentCount { get; set; }

            [Required]
            public string Description { get; set; }
        }
    }
}
