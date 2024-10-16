﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API.Context;
using API.Models;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserDevicesController : ControllerBase
    {
        private readonly AppDBContext _context;

        public UserDevicesController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/UserDevices
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserDevice>>> GetUserDevice()
        {
            return await _context.UserDevice.ToListAsync();
        }

        // GET: api/UserDevices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserDevice>> GetUserDevice(string id)
        {
            var userDevice = await _context.UserDevice.FindAsync(id);

            if (userDevice == null)
            {
                return NotFound();
            }

            return userDevice;
        }

        // PUT: api/UserDevices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserDevice(string id, UserDevice userDevice)
        {
            if (id != userDevice.Id)
            {
                return BadRequest();
            }

            _context.Entry(userDevice).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserDevices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserDevice>> PostUserDevice(UserDeviceDTO userDeviceDTO)
        {
            var userDevice = new UserDevice
            {
                Id = Guid.NewGuid().ToString("N"),
                UserId = userDeviceDTO.UserID,
                DeviceId = userDeviceDTO.DeviceId,
            };

            _context.UserDevice.Add(userDevice);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (UserDeviceExists(userDevice.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetUserDevice", new { id = userDevice.Id }, userDevice);
        }

        // DELETE: api/UserDevices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserDevice(string id)
        {
            var userDevice = await _context.UserDevice.FindAsync(id);
            if (userDevice == null)
            {
                return NotFound();
            }

            _context.UserDevice.Remove(userDevice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserDeviceExists(string id)
        {
            return _context.UserDevice.Any(e => e.Id == id);
        }
    }
}
