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
using Rockstars.Classes;
using Rockstars.Filter;
using Rockstars.Models;

namespace Rockstars.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ApiKeyAuth]
    public class SongsController : ControllerBase
    {
        private readonly SongContext _context;
        private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        public SongsController(SongContext context)
        {
            _context = context;
        }
        #region get
        // GET: api/Songs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Song>>> GetSongs()
        {
            return await _context.Songs.ToListAsync();
        }

        // GET: api/Songs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Song>> GetSong(int id)
        {
            var song = await _context.Songs.FindAsync(id);

            if (song == null)
            {
                return NotFound();
            }
            return song;
        }

        [HttpGet()]
        [Route("~/api/songbyparameter/{parameter}")]
        public ActionResult<IEnumerable<Song>> GetSongByParameter(string parameter)
        {
            try
            {
                parameter = parameter.ToLower();
                var songs = _context.Songs.Where(
                    song => song.Artist.Contains(parameter) ||
                    song.Genre.Contains(parameter) ||
                    song.Name.Contains(parameter) ||
                    song.Album.Contains(parameter)
                ).ToList();
                if (songs.Count == 0)
                {
                    throw new ObjectNotFoundException(string.Format(Constant.SongNotFound, parameter));
                }
                return songs;
            }
            catch (ObjectNotFoundException ex)
            {
                log.Error(ex.Message);
                return StatusCode(406, ex.Message);
            }
            catch (Exception ex)
            {
                log.Error(Constant.UnknownException + ex.Message);
                return NotFound();
            }
        }
        #endregion
        #region put
        // PUT: api/Songs/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Song>> PutSong(int id, Song song)
        {
            try
            {
                if (id != song.Id)
                {
                    return BadRequest();
                }
                _context.Entry(song).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return song;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SongExists(id))
                {
                    log.Error(Constant.UnknownException);
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        #endregion
        #region post

        // POST: api/Songs
        [HttpPost("{year:int?}")]
        public async Task<ActionResult<Song>> PostSongs(List<Song> songs, int? year = null, [FromQuery] string genre = "")
        {
            try
            {
                List<Song> songList = FilterSongs(songs, genre, year);
                _context.Songs.AddRange(songList);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetSong", new { id = songList[0].Id }, songList);
            }
            catch (Exception ex)
            {
                log.Error(Constant.UnknownException + ex.Message);
                throw ex;
            }
        }

        [HttpPost]
        [Route("~/api/postnewsong")]
        public async Task<ActionResult<Song>> PostNewSong(Song song)
        {
            try
            {
                if (!CheckIfSongExists(song))
                {
                    _context.Songs.Add(song);
                    await _context.SaveChangesAsync();
                    return CreatedAtAction("GetSong", new { id = song.Id }, song);
                }
                else
                {
                    throw new ConstraintException(Constant.SongException);
                }
            }
            catch (ConstraintException ex)
            {
                log.Error(ex.Message);
                return StatusCode(406, ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                log.Error(ex.Message);
                return StatusCode(406, Constant.UniqueViolation);
            }
            catch (Exception ex)
            {
                log.Error(Constant.UnknownException + ex.Message);
                return StatusCode(406, ex.Message);
            }
        }
        #endregion
        #region delete
        // DELETE: api/Songs/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Song>> DeleteSong(int id)
        {
            try
            {
                var song = await _context.Songs.FindAsync(id);
                if (song == null)
                {
                    throw new ObjectNotFoundException(string.Format(Constant.SongIdNotFound, id.ToString()));
                }

                _context.Songs.Remove(song);
                await _context.SaveChangesAsync();

                return song;
            }
            catch (ObjectNotFoundException ex)
            {
                log.Error(ex.Message);
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                log.Error(Constant.UnknownException + ex.Message);
                return NotFound();
            }
        }
        #endregion
        #region private
        private bool SongExists(int id)
        {
            return _context.Songs.Any(e => e.Id == id);
        }
        private List<Song> FilterSongs(List<Song> songs, string genre, int? year)
        {
            try
            {
                if (year == null)
                {
                    year = 2020;
                }
                if (genre == null)
                {
                    genre = string.Empty;
                }
                List<Song> filteredList = songs.Where(
                    song => song.Genre.Contains(genre.ToLower()) &&
                    song.Year <= year).OrderBy(song => song.Year
                    ).ToList();
                return filteredList;
            }
            catch (Exception)
            {
                log.Error(Constant.SortException);
                throw new Exception(Constant.SortException);
            }
        }

        private bool CheckIfSongExists(Song song)
        {
            List<Song> songs = _context.Songs.ToList();
            bool hasSong = songs.Any(x => x.Name == song.Name);
            return hasSong;
        }
        #endregion
    }
}
