#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;
using ContosoUniversity.Models;

namespace ContosoUniversity.Pages.Departments
{
    public class EditModel : PageModel
    {
        private readonly ContosoUniversity.Data.SchoolContext _context;

        public EditModel(ContosoUniversity.Data.SchoolContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Department Department { get; set; }

        public SelectList InstructorNameSL { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Department = await _context.Departments
                .AsNoTracking()
                .Include(d => d.Administrator)
                .FirstOrDefaultAsync(m => m.DepartmentID == id);

            if (Department == null)
            {
                return NotFound();
            }

            // Use strongly typed data rather than ViewData
            InstructorNameSL = new SelectList(_context.Instructors, "ID", "FullName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var departmentToUpdate = await _context.Departments
                .Include(i => i.Administrator)
                .FirstOrDefaultAsync(m => m.DepartmentID == id);

            if (departmentToUpdate == null)
            {
                return HandleDeleteDepartment();
            }

            // Set ConcurrencyToken to value read in OnGetAsync
            _context.Entry(departmentToUpdate)
                .Property(e => e.ConcurrencyToken).OriginalValue = Department.ConcurrencyToken;

            if (await TryUpdateModelAsync(
                departmentToUpdate,
                "Department",
                s => s.Name, s => s.Budget, s => s.InstructorID, s => s.StartDate))
            {
                try
                {
                    await _context.SaveChangesAsync();
                    return RedirectToPage("./Index");
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    var exceptionEntry = ex.Entries.Single();
                    var clientValues = (Department)exceptionEntry.Entity;
                    var databaseEntry = exceptionEntry.GetDatabaseValues();
                    if (databaseEntry == null)
                    {
                        ModelState.AddModelError(string.Empty, "Unable to save. " +
                            "The department was deleted by another user.");
                        return Page();
                    }

                    var dbValues = (Department)databaseEntry.ToObject();
                    await setDbErrorMessage(dbValues, clientValues, _context);

                    // Save the current ConcurrencyToken so next postback
                    // matches unless a new concurrency issue happens.
                    Department.ConcurrencyToken = (byte[])dbValues.ConcurrencyToken;
                    // Clear the model error for the next postback
                    ModelState.Remove($"{nameof(Department)}.{nameof(Department.ConcurrencyToken)}");
                }
            }

            InstructorNameSL = new SelectList(_context.Instructors, "ID", "FullName", departmentToUpdate);
            return Page();
        }

        private IActionResult HandleDeleteDepartment()
        {
            var deletedDepartment = new Department();
            // ModelState contains the posted data because of the deletion error
            // and overrides the Department instance values when deisplaying Page().
            ModelState.AddModelError(string.Empty,
                "Unable to Save. The department ws deleted by another user.");
            InstructorNameSL = new SelectList(_context.Instructors, "ID", "FullName", Department.InstructorID);
            return Page();
        }

        private async Task setDbErrorMessage(Department dbValues, Department clientValues, SchoolContext context)
        {
            if (dbValues.Name != clientValues.Name)
            {
                ModelState.AddModelError("Department.Name", $"Current value: {dbValues.Name}");
            }
            if (dbValues.Budget != clientValues.Budget)
            {
                ModelState.AddModelError("Department.Budget", $"Current value: {dbValues.Budget}");
            }
            if (dbValues.StartDate != clientValues.StartDate)
            {
                ModelState.AddModelError("Department.StartDate", $"Current value: {dbValues.StartDate}");
            }
            if (dbValues.InstructorID != clientValues.InstructorID)
            {
                Instructor dbInstructor = await _context.Instructors
                    .FindAsync(dbValues.InstructorID);
                ModelState.AddModelError("Department.Instructor", $"Current value: {dbInstructor.FullName}");
            }

            ModelState.AddModelError(string.Empty,
                "The record you attempted to edit "
                + "was modified by another user after you. The "
                + "edit operation was canceled and the current values in the database "
                + "have been displayed. If you still want to edit this record, click "
                + "the Save button again.");
        }


    }
}
