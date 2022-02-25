using ContosoUniversity.Data;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace ContosoUniversity.Pages
{
    public class DepartmentNamePageModel : PageModel
    {
        public SelectList? DepartmentNameSL { get; set; }

        public void PopulateDepartmentsDropdownList(SchoolContext _context, object? selectedDepartment = null)
        {
            var departmentIQ = from d in _context.Departments
                               orderby d.Name // Sort by Name
                               select d;

            DepartmentNameSL = new SelectList(departmentIQ.AsNoTracking(),
                "DepartmentID", "Name", selectedDepartment);
        }
    }
}
