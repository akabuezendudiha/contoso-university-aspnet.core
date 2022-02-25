#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ContosoUniversity.Models.ViewModels
{
    public class CourseViewModel
    {
        [Display(Name = "Number")]
        public int CourseID { get; set; }
        
        [Display(Name = "Title")]
        public string Title { get; set; }
        
        [Display(Name = "Credits")]
        public int Credits { get; set; }

        [Display(Name = "Department")]
        public string DepartmentName { get; set; }
    }
}
