using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Rockstars.Classes;
using Rockstars.Filter;
using Rockstars.Models;

namespace Rockstars.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuth]
    public class BandsController : ControllerBase
    {
        private readonly BandContext _context;
        private readonly JWTToken jwtToken = new JWTToken();
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public BandsController(BandContext context)
        {
            _context = context;
        }

        [HttpPost]
        [Route("~/api/authenticate/{email}")]
        public string Authenticate(string email)
        {
            AuthenticateResponse response = jwtToken.Authenticate(email);
            return "ok";
        }

        #region Get
        // GET: api/Bands
        public async Task<ActionResult<IEnumerable<Band>>> GetBands()
        {
            return await _context.Bands.ToListAsync();
        }

        // GET: api/Bands/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Band>> GetBand(int id)
        {
            try
            {
                var band = await _context.Bands.FindAsync(id);
                if (band == null)
                {
                    throw new ObjectNotFoundException(string.Format(Constant.BandIdNotFound, id.ToString()));
                }
                return band;
            }
            catch (ObjectNotFoundException ex)
            {
                log.Error(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                log.Error(Constant.UnknownException);
                return NotFound();
            }
        }

        [HttpGet()]
        [Route("~/api/bandbyname/{name}")]
        public ActionResult<IEnumerable<Band>> GetBandByName(string name)
        {
            try
            {
                var bands = _context.Bands.Where(band => band.Name.Contains(name.ToLower())).ToList();
                if (bands.Count == 0)
                {
                    throw new ObjectNotFoundException(string.Format(Constant.BandNotFound, name));
                }
                return bands;
            }
            catch (ObjectNotFoundException ex)
            {
                log.Error(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                log.Error(Constant.UnknownException);
                return NotFound();
            }
        }
        #endregion
        #region Put
        // PUT: api/Bands/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Band>> PutBand(int id, Band band)
        {
            if (id != band.Id)
            {
                return BadRequest();
            }
            band.Name = band.Name.ToLower();
            _context.Entry(band).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BandExists(id))
                {
                    log.Error(Constant.UnknownException);
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return band;
        }
        #endregion
        #region Post
        // POST: api/Bands
        [HttpPost]
        public async Task<ActionResult<Band>> PostBand(List<Band> bands)
        {
            try
            {
                List<Band> bandList = SortBands(bands);
                _context.Bands.AddRange(bandList);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetBand", new { id = bands[0].Id }, bands);
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        [Route("~/api/postnewband")]
        public async Task<ActionResult<Band>> PostNewBand(Band band)
        {
            try
            {
                if (!CheckIfBandExists(band))
                {
                    _context.Bands.Add(band);
                    await _context.SaveChangesAsync();

                    return CreatedAtAction("GetBand", new { id = band.Id }, band);
                }
                else
                {
                    throw new ConstraintException(Constant.BandException);
                }
            }
            catch(ConstraintException ex)
            {
                log.Error(ex.Message);
                return StatusCode(406, ex.Message);
            }
            catch (InvalidOperationException)
            {
                log.Error(Constant.UniqueViolation);
                return StatusCode(406, Constant.UniqueViolation);
            }
            catch (Exception ex)
            {
                log.Error(Constant.UnknownException + ex.Message);
                return StatusCode(406, ex.Message);
            }
        }
        #endregion
        #region Delete
        // DELETE: api/Bands/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Band>> DeleteBand(int id)
        {
            try
            {
                var band = await _context.Bands.FindAsync(id);
                if (band == null)
                {
                    throw new ObjectNotFoundException(string.Format(Constant.BandIdNotFound, id.ToString()));
                }
                _context.Bands.Remove(band);
                await _context.SaveChangesAsync();

                return band;
            }
            catch (ObjectNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
        #endregion
        #region Private
        private bool BandExists(int id)
        {
            return _context.Bands.Any(e => e.Id == id);
        }

        /// <summary>
        /// Sort bands by ID and removing bands with id 0
        /// </summary>
        /// <param name="bands"></param>
        /// <returns></returns>
        private List<Band> SortBands(List<Band> bands)
        {
            foreach (Band band in bands)
            {
                band.Name = band.Name.ToLower();
            }
            bands = bands.Where(band => band.Id != 0).OrderBy(x => x.Id).ToList();
            return bands;
        }

        private bool CheckIfBandExists(Band band)
        {
            List<Band> bands = _context.Bands.ToList();
            bool hasBand = bands.Any(x => x.Name == band.Name);
            return hasBand;
        }
        #endregion
    }
}
