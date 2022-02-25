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

namespace ContosoUniversity.Pages.Instructors
{
    public class IndexModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public IndexModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        [BindProperty(SupportsGet = true)]  
        public int? InstructorID { get; set; }

        [BindProperty(SupportsGet = true)]
        public int? CourseID { get; set; }

        public InstructorIndexData InstructorData { get;set; } 

        public async Task OnGetAsync(int? id, int? courseId)
        {
            InstructorData = new InstructorIndexData(); // Initialize

            // Loads the instructors
            InstructorData.Instructors = await _context.Instructors 
                .Include(x => x.Courses)
                    .ThenInclude(c => c.Department)
                .Include(x => x.OfficeAssignment)
                .ToListAsync();

            // Loads Instructor Courses  
            if (id != null)
            {
                InstructorID = id;
                var instructor = InstructorData.Instructors
                    .Where(c => c.ID == id).Single();
                InstructorData.Courses = instructor.Courses;
            }

            // Loads course enrollments
            if (courseId != null)
            {
                CourseID = courseId;
                var selectedCourse = InstructorData.Courses
                    .Where(c => c.CourseID == courseId).Single();
                await _context.Entry(selectedCourse)
                    .Collection(c => c.Enrollments)
                    .LoadAsync();
                foreach (var enrollment in selectedCourse.Enrollments)
                {
                    await _context.Entry(enrollment)
                        .Reference(x => x.Student)
                        .LoadAsync();
                }
                InstructorData.Enrollments = selectedCourse.Enrollments;
            }            
        }
    }
}
