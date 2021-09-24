using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PassportBooking.Data;
using PassportBooking.Models.Entities;
using PassportBooking.Models.ViewModels.GymClass;
using PassportBooking.Areas.Identity.Pages.Account;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using PassportBooking.Web.Extensions;

namespace PassportBooking.Controllers
{
    public class GymClassesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ApplicationDbContext db;
        private IQueryable <GymClass> gymClass;

        public GymClassesController(ApplicationDbContext context, IMapper mapper, UserManager<ApplicationUser> userManager)
        {
            db = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        // GET: GymClasses
        public async Task<IActionResult> Index(string passType)
        {

            if (passType=="Now")
            {
                gymClass = db.GymClass.Include(m => m.AttendingMembers).Where(g => g.StartTime >= DateTime.Now
                    && db.ApplicationUserGymclass.Where(u => u.ApplicationUserId == userManager.GetUserId(User)).Any(u => u.GymclassId == g.Id));
                ViewData["TempTitle"] = "Booked Passes";
            }
            else if (passType == "Old")
            {
                gymClass = db.GymClass.Include(m => m.AttendingMembers).Where(g => g.StartTime < DateTime.Now
                       && db.ApplicationUserGymclass.Where(u => u.ApplicationUserId == userManager.GetUserId(User)).Any(u => u.GymclassId == g.Id));
                ViewData["TempTitle"] = "History";
            }
            else if (passType == "Previos")
            {
                gymClass = db.GymClass.Include(m => m.AttendingMembers).Where(g => g.StartTime < DateTime.Now
                     && !db.ApplicationUserGymclass.Where(u => u.ApplicationUserId == userManager.GetUserId(User)).Any(u => u.GymclassId == g.Id));
                ViewData["TempTitle"] = "Previos Passes";
            }
            else
            {               
               gymClass = db.GymClass.Include(m => m.AttendingMembers).Where(g => g.StartTime >= DateTime.Now 
                    && ! db.ApplicationUserGymclass.Where(u => u.ApplicationUserId == userManager.GetUserId(User)).Any(u => u.GymclassId==g.Id));
                ViewData["TempTitle"] = "Current Passes";
            }
            var model = _mapper.ProjectTo<GymClassIndexViewModel>(gymClass);
            if (model == null)
            {
                return NotFound();
            }

            return View( await model.ToListAsync());
        }

        // GET: GymClasses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = db.GymClass.Include(m => m.AttendingMembers);


           var members =await db.ApplicationUser.Where(m => 
                     db.ApplicationUserGymclass.Where(u => u.GymclassId == id).Any(u => u.ApplicationUserId == m.Id)).ToListAsync();

            var model = await _mapper.ProjectTo<GymClassIndexViewModel>(gymClass)
                .FirstOrDefaultAsync(m => m.Id == id);

            model.Members = members;

            if (gymClass == null)
            {
                return NotFound();
            }

            return Request.IsAjax() ? PartialView("DetailsPartial", model) : View(model);
        }

        // GET: GymClasses/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: GymClasses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]

        public async Task<IActionResult> Create([Bind("Id,Name,StartTime,Duration,Description")] GymClass gymClass)
        {
            if (ModelState.IsValid)
            {
                db.Add(gymClass);
                await db.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(gymClass);
        }

        // GET: GymClasses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await db.GymClass.FindAsync(id);
            if (gymClass == null)
            {
                return NotFound();
            }
            return View(gymClass);
        }

        // POST: GymClasses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int? id)
        {
            if (id is null)
            {
                return BadRequest();
            }
                var gymClass = db.GymClass.Find(id);
                if (await TryUpdateModelAsync(gymClass, "", g => g.Name, g => g.Duration, g => g.StartTime, g => g.Description))
                {
                    try
                    {
                        await db.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {

                        if (!GymClassExists(gymClass.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction(nameof(Index));
                }
            return View(gymClass);
        }

        // GET: GymClasses/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var gymClass = await db.GymClass
                .FirstOrDefaultAsync(m => m.Id == id);
            if (gymClass == null)
            {
                return NotFound();
            }

            return View(gymClass);
        }

        // POST: GymClasses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var gymClass = await db.GymClass.FindAsync(id);
            db.GymClass.Remove(gymClass);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool GymClassExists(int id)
        {
            return db.GymClass.Any(e => e.Id == id);
        }
        [Authorize]
        public async Task<IActionResult> BookingToggle(int? id)
        {
            if (id == null)
            {
                return BadRequest();
            }
            var userId = userManager.GetUserId(User);

            var attending =await db.ApplicationUserGymclass.FindAsync(userId,id);
            if (attending is null)
            {
                ApplicationUserGymclass pass = new ApplicationUserGymclass
                {
                    ApplicationUserId = userId,
                    GymclassId = (int)id
                };
                db.ApplicationUserGymclass.Add(pass);
                TempData["Message"] = "Booking";
            }
            else
            {
                db.ApplicationUserGymclass.Remove(attending);
                TempData["Message"] = "Unbooking";
            }
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
