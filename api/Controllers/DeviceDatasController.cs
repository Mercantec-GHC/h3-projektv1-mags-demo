using System;
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
    public class DeviceDatasController : ControllerBase
    {
        private readonly AppDBContext _context;

        public DeviceDatasController(AppDBContext context)
        {
            _context = context;
        }

        // GET: api/DeviceDatas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DeviceDataReadDTO>>> GetDeviceData()
        {
            return await _context.DeviceData
                .Select(dd => new DeviceDataReadDTO
                {
                    Id = dd.Id,
                    DeviceId = dd.DeviceId,
                    CreatedAt = dd.CreatedAt,
                    Temperature = dd.Temperature,
                    Humidity = dd.Humidity,
                    GasResistor = dd.GasResistor,
                    VolatileOrganicCompounds = dd.VolatileOrganicCompounds,
                    CO2 = dd.CO2
                })
                .ToListAsync();
        }

        // GET: api/DeviceDatas/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DeviceDataReadDTO>> GetDeviceData(string id)
        {
            var deviceData = await _context.DeviceData.FindAsync(id);

            if (deviceData == null)
            {
                return NotFound();
            }

            var deviceDataDTO = new DeviceDataReadDTO
            {
                Id = deviceData.Id,
                DeviceId = deviceData.DeviceId,
                CreatedAt = deviceData.CreatedAt,
                Temperature = deviceData.Temperature,
                Humidity = deviceData.Humidity,
                GasResistor = deviceData.GasResistor,
                VolatileOrganicCompounds = deviceData.VolatileOrganicCompounds,
                CO2 = deviceData.CO2
            };

            return deviceDataDTO;
        }

        // PUT: api/DeviceDatas/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDeviceData(string id, DeviceDataReadDTO deviceDataDTO)
        {
            if (id != deviceDataDTO.Id)
            {
                return BadRequest();
            }

            var deviceData = await _context.DeviceData.FindAsync(id);
            if (deviceData == null)
            {
                return NotFound();
            }

            deviceData.Temperature = deviceDataDTO.Temperature;
            deviceData.Humidity = deviceDataDTO.Humidity;
            deviceData.GasResistor = deviceDataDTO.GasResistor;
            deviceData.VolatileOrganicCompounds = deviceDataDTO.VolatileOrganicCompounds;
            deviceData.CO2 = deviceDataDTO.CO2;
            deviceData.UpdatedAt = DateTime.UtcNow;

            _context.Entry(deviceData).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceDataExists(id))
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

        // POST: api/DeviceDatas
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DeviceDataReadDTO>> PostDeviceData(DeviceDataCreateDTO deviceDataCreateDTO)
        {
            var deviceData = new DeviceData
            {
                Id = Guid.NewGuid().ToString("N"),
                DeviceId = deviceDataCreateDTO.DeviceId,
                Temperature = deviceDataCreateDTO.Temperature,
                Humidity = deviceDataCreateDTO.Humidity,
                GasResistor = deviceDataCreateDTO.GasResistor,
                VolatileOrganicCompounds = deviceDataCreateDTO.VolatileOrganicCompounds,
                CO2 = deviceDataCreateDTO.CO2,
                CreatedAt = DateTime.UtcNow.AddHours(2),
                UpdatedAt = DateTime.UtcNow.AddHours(2)
            };

            _context.DeviceData.Add(deviceData);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (DeviceDataExists(deviceData.Id))
                {
                    return Conflict();
                }
                else
                {
                    throw;
                }
            }

            var deviceDataReadDTO = new DeviceDataReadDTO
            {
                DeviceId = deviceData.DeviceId,
                CreatedAt = deviceData.CreatedAt,
                Temperature = deviceData.Temperature,
                Humidity = deviceData.Humidity,
                GasResistor = deviceData.GasResistor,
                VolatileOrganicCompounds = deviceData.VolatileOrganicCompounds,
                CO2 = deviceData.CO2
            };

            return CreatedAtAction("GetDeviceData", new { id = deviceData.Id }, deviceDataReadDTO);
        }

        // DELETE: api/DeviceDatas/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDeviceData(string id)
        {
            var deviceData = await _context.DeviceData.FindAsync(id);
            if (deviceData == null)
            {
                return NotFound();
            }

            _context.DeviceData.Remove(deviceData);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DeviceDataExists(string id)
        {
            return _context.DeviceData.Any(e => e.Id == id);
        }
    }
}

