#nullable disable
using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ContosoUniversity.Pages.Instructors
{
    public class InstructorCoursesPageModel : PageModel
    {
        public List<AssignedCourseData> AssignedCoursesDataList { get; set; }

        public void PopulateAssginedCourseData(SchoolContext _context, Instructor instructor)
        {
            var allCourses = _context.Courses;
            var instructorCourses = new HashSet<int>(
                instructor.Courses.Select(c => c.CourseID));
            AssignedCoursesDataList = new List<AssignedCourseData>();
            foreach (var course in allCourses)
            {
                AssignedCoursesDataList.Add(new AssignedCourseData
                {
                    CourseID = course.CourseID,
                    Title = course.Title,
                    Assigned = instructorCourses.Contains(course.CourseID)
                });
            }
        }
    }
}
