using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ContactProAltair.Data;
using ContactProAltair.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using ContactProAltair.Services.Interfaces;
using ContactProAltair.Enums;

namespace ContactProAltair.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IImageService _imageService;
        private readonly IAddressBookService _addressBookService;

        public ContactsController(ApplicationDbContext context, UserManager<AppUser> userManager, IImageService imageService, IAddressBookService addressBookService)
        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
            _addressBookService = addressBookService;

        }

        // GET: Contacts
        public async Task<IActionResult> Index(int? categoryId)
        {
            string? userId = _userManager.GetUserId(User);

            List<Contact> contacts = await _context.Contacts.Include(c => c.Categories).Where(c => c.AppUserID == userId).ToListAsync();

            List<Contact> model = new List<Contact>();

            if (categoryId != null)
            {
                model = (await _context.Categories.Include(c => c.Contacts).FirstOrDefaultAsync(c => c.Id == categoryId))!.Contacts.ToList();
            }
            else
            {
                model = contacts.OrderBy(c => c.LastName).ThenBy(c => c.FirstName).ToList();
            }

            List<Category> categories = await _context.Categories.Where(c => c.AppUserId == userId).ToListAsync();

            ViewData["CategoriesList"] = new SelectList(categories, "Id", "Name", categoryId);

            return View(model);
        }

        public async Task<IActionResult> SearchContacts(string? searchString)
        {
            string? userId = _userManager.GetUserId(User);

            List<Contact> contacts = await _context.Contacts.Include(c => c.Categories).Where(c => c.AppUserID == userId).ToListAsync();

            List<Contact> model = new List<Contact>();

            if (string.IsNullOrEmpty(searchString))
            {
                model = contacts;
            }
            else
            {
                model = contacts.Where(c => c.FullName!.ToLower().Contains(searchString.ToLower()))
                                .OrderBy(c => c.LastName)
                                .ThenBy(c => c.FirstName)
                                .ToList();
            }

            List<Category> categories = await _context.Categories.Where(c => c.AppUserId == userId).ToListAsync();

            ViewData["CategoriesList"] = new MultiSelectList(categories, "Id", "Name");

            ViewData["SearchString"] = searchString;

            return View(nameof(Index), model);
        }

        [HttpGet]
        public async Task<IActionResult> EmailContact(int? id)
        {
            if(id == null)
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmailContact(EmailContactViewModel viewModel)
        {
            if(ModelState.IsValid)
            {

            }

            return View(viewModel);
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        [Authorize]
        public async Task<IActionResult> Create()
        {
            Contact contact = new Contact();

            string userId = _userManager.GetUserId(User)!;

            List<Category> categories = await _context.Categories.Where(c => c.AppUserId == userId).ToListAsync();

            ViewData["CategoriesList"] = new MultiSelectList(categories, "Id", "Name");

            ViewData["StatesList"] = new SelectList(Enum.GetValues(typeof(States)).Cast<States>());


            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FirstName,LastName,DateOfBirth,Address1,Address2,City,State,ZipCode,Email,PhoneNumber,ImageFile")] Contact contact, List<int> selected)
        {
            ModelState.Remove("AppUserID");

            if (ModelState.IsValid)
            {


                //Set user id
                contact.AppUserID = _userManager.GetUserId(User);

                //Set created date
                contact.CreatedDate = DateTime.Now;

                //Set the image data if one has been chosen
                if (contact.ImageFile != null)
                {
                    //create image service, convert file to byte array and assign it to the imagedata
                    contact.ImageData = await _imageService.ConvertFileToByteArrayAsync(contact.ImageFile);

                    //Assign the imagetype based on the chosen file
                    contact.ImageType = contact.ImageFile.ContentType;
                }

                _context.Add(contact);
                await _context.SaveChangesAsync();

                await _addressBookService.AddCategoriesToContactAsync(selected, contact.Id);

                return RedirectToAction(nameof(Index));
            }

            return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            string userId = _userManager.GetUserId(User)!;

            Contact? contact = await _context.Contacts.Include(c => c.Categories).FirstOrDefaultAsync(c => c.Id == id && c.AppUserID == userId);

            if (contact == null)
            {
                return NotFound();
            }

            //add viewdata for catagories
            List<Category> categories = await _context.Categories.Where(c => c.AppUserId == userId).ToListAsync();

            List<int> categoryIds = contact.Categories.Select(c => c.Id).ToList();

            ViewData["CategoryList"] = new MultiSelectList(categories, "Id", "Name", categoryIds);

            // add viewdata for states
            ViewData["StatesList"] = new SelectList(Enum.GetValues(typeof(States)).Cast<States>());

            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserID,FirstName,LastName,CreatedDate,DateOfBirth,Address1,Address2,City,State,ZipCode,Email,PhoneNumber,ImageFile,ImageData,ImageType")] Contact contact, List<int> selected)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    //Set the image data if one has been chosen
                    if (contact.ImageFile != null)
                    {
                        //create image service, convert file to byte array and assign it to the imagedata
                        contact.ImageData = await _imageService.ConvertFileToByteArrayAsync(contact.ImageFile);

                        //Assign the imagetype based on the chosen file
                        contact.ImageType = contact.ImageFile.ContentType;
                    }

                    _context.Update(contact);
                    await _context.SaveChangesAsync();

                    await _addressBookService.RemoveCategoriesFromContactAsync(contact.Id);

                    await _addressBookService.AddCategoriesToContactAsync(selected, contact.Id);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
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


            // add viewdata for states
            return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Contacts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Contacts'  is null.");
            }
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return (_context.Contacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
