using ContactProAltair.Data;
using ContactProAltair.Models;
using ContactProAltair.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ContactProAltair.Services
{
    public class AddressBookService : IAddressBookService
    {
        private readonly ApplicationDbContext _context;

        public AddressBookService(ApplicationDbContext context) 
        {
            _context = context; 
        
        }

        public async Task AddCategoriesToContactAsync(List<int> categoryIds, int contactId)
        {
            try
            {
                //get contact to add categories to
                Contact? contact = await _context.Contacts.Include(c => c.Categories).FirstOrDefaultAsync(c => c.Id == contactId);

                //if this contact doesnt exist, just stop now
                if (contact == null)
                {
                    return;
                }

                //loop thorugh each category id
                foreach(int categoryId in categoryIds) 
                {
                    //make sure category exists
                    Category? category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == categoryId);

                    //if it does add the contact to the category
                    if(category != null) 
                    {
                        contact.Categories.Add(category);
                    }

                }

                //then, save changes to datebase
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task RemoveCategoriesFromContactAsync(int contactId)
        {
            try
            {
                //find the contact by id
                Contact? contact = await _context.Contacts.Include(c => c.Categories).FirstOrDefaultAsync(c => c.Id == contactId);

                if(contact != null)
                {
                    //remove all of their categories
                    contact.Categories.Clear();
                }
               
                //save changes to datebase
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
