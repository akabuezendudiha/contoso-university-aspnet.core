#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.ViewModels;

namespace ContosoUniversity.Pages.Courses
{
    public class IndexSelectModelModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public IndexSelectModelModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        public IList<CourseViewModel> CourseVM { get;set; }

        public async Task OnGetAsync()
        {
            CourseVM = await _context.Courses                
                .Include(c => c.Department)
                .Select(c => new CourseViewModel
                {
                    CourseID = c.CourseID,  
                    Title = c.Title,
                    Credits = c.Credits,
                    DepartmentName = c.Department.Name,
                })
                .ToListAsync();
        }
    }
}
