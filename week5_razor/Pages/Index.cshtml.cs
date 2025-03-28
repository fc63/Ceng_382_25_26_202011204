/*
prompt: "I’m building a Razor Pages web app to manage a list of classes.

I want the backend (Index.cshtml.cs) to:

    Store class data in a simple static list (in memory).

    Each class has: Id (auto-increment), Class Name, Student Count, and Description.

    I need a form that allows adding a new class, editing an existing one, and deleting.

The page should support:

    Form validation: make Class Name and Description required, and Student Count should be between 1 and 500.

    If I'm editing, the form should show the current values.

    After submitting (add, edit, or delete), it should refresh the page.

Use a nested class for form input if needed.
The code should use C# and Razor Pages only (no JavaScript).
Keep it clean and simple."
*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using week5_razor.Models;

namespace week5_razor.Pages
{
    public class IndexModel : PageModel
    {
        public static List<ClassInformationModel> ClassList { get; set; } = new List<ClassInformationModel>();

        [BindProperty]
        public ClassInputModel Input { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? EditId { get; set; }

        public bool IsEdit => EditId.HasValue;

        public void OnGet()
        {
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
