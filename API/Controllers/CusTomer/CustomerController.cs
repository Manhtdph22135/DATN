using API.Context;
using API.DOT;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.CusTomer
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {
        private readonly DbContextShop _contextShop;

        public CustomerController(DbContextShop contextShop)
        {
            _contextShop = contextShop;
        }

        // GET: api/Customer
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Customer>>> GetCustomers()
        {
            return await _contextShop.Customers.ToListAsync();
        }
        [HttpGet("get-customer-by-account/{accountId}")]
        public async Task<ActionResult<object>> GetCustomerByAccount(int accountId)
        {
            var customer = await (from c in _contextShop.Customers
                                  join a in _contextShop.Accounts on c.AccountId equals a.AccountId
                                  where c.AccountId == accountId
                                  select new
                                  {
                                        c.CustomerId,
                                        c.FullName,
                                        c.Email,
                                        c.Phone,
                                        c.Gender,
                                        c.Dob,
                                        c.Address,
                                        c.CreateAt,
                                        c.UpdateAt,
                                        c.RankMember,
                                        c.Point,
                                        a.Username,
                                        a.PasswordHash,
                                        a.AccountId,
                                        a.RoleId
                                  }).FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound("Customer not found for the given account ID.");
            }
            return Ok(customer);
        }

        // GET: api/Customer/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Customer>> GetCustomer(int id)
        {
            var customer = await _contextShop.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound();
            }

            return customer;
        }

        // PUT: api/Customer/5
        [HttpPut("update-account-customer")]
        public async Task<IActionResult> UpdateAccountCustomer([FromBody] UpdateAccountCustomerDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingAcc = await _contextShop.Accounts.FindAsync(dto.AccountId);
            if (existingAcc == null)
                return NotFound(new { message = "Tài khoản không tồn tại!" });

            // Nếu muốn check mật khẩu cũ (chưa hash):
            if (!string.IsNullOrEmpty(dto.CurrentPassword) &&
                existingAcc.PasswordHash != dto.CurrentPassword)
            {
                return BadRequest(new { message = "Mật khẩu hiện tại không đúng!" });
            }

            // TODO: nếu dùng hash thì hash tại đây
            existingAcc.PasswordHash = dto.NewPassword;

            _contextShop.Accounts.Update(existingAcc);
            await _contextShop.SaveChangesAsync();

            return Ok(new { message = "Cập nhật mật khẩu tài khoản khách hàng thành công!" });
        }


        // POST: api/Customer
        [HttpPost]
        public async Task<ActionResult<Customer>> PostCustomer(Customer customer)
        {
            _contextShop.Customers.Add(customer);
            await _contextShop.SaveChangesAsync();

            return CreatedAtAction("GetCustomer", new { id = customer.CustomerId }, customer);
        }

        // DELETE: api/Customer/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            var customer = await _contextShop.Customers.FindAsync(id);
            if (customer == null)
            {
                return NotFound("Không tìm thấy id khách hàng");
            }

            _contextShop.Customers.Remove(customer);
            await _contextShop.SaveChangesAsync();

            return Ok("Xoá Thành Công");
        }

        private bool CustomerExists(int id)
        {
            return _contextShop.Customers.Any(e => e.CustomerId == id);
        }
    }
}
