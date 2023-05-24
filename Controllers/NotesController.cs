using DiplomaWeb.Db;
using DiplomaWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using static DiplomaWeb.Extensions.HttpExtensions;

namespace DiplomaWeb.Controllers {
    [ApiController]
    [Route("api/[controller]")]
    public class NotesController : ControllerBase {
        private readonly ApplicationContext _context;



        public NotesController(ApplicationContext context) {
            _context = context;
        }



        [HttpGet("Ping")]
        public async Task<ActionResult<string>> Ping() {
            return new ObjectResult("Pong.");
        }

        [Authorize]
        [HttpGet("Notes")]
        public async Task<List<Note>> Notes() {
            string username = HttpContext.GetName();
            return _context.Notes.Where(note => note.UserName == username).ToList();
        }

        [Authorize]
        [HttpGet("Note")]
        public async Task<ActionResult<Note>> GetNote(string branchName, int version) {
            string username = HttpContext.GetName();
            Note? note = await _context.Notes.FirstOrDefaultAsync(note => note.UserName == username && note.Version == version && note.BranchName == branchName);
            if (note == null) {
                return NotFound();
            }

            return new ObjectResult(note);
        }

        [Authorize]
        [HttpPost("Note")]
        public async Task<ActionResult> PostNote(Note postNote) {
            if (postNote.Name == null) {
                return UnprocessableEntity(postNote);
            }

            if (postNote.Name == "") {
                return UnprocessableEntity(postNote);
            }

            if (postNote.Text == null) {
                return UnprocessableEntity(postNote);
            }

            try {
                await PrepareNote();
            }
            catch (PrepareNoteException) {
                return UnprocessableEntity(postNote);
            }

            _context.Notes.Add(postNote);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(PostNote), postNote);


            async Task PrepareNote() {
                string userName = HttpContext.GetName();
                User user = _context.Users.First((user) => user.Login == userName);
                postNote.UserName = user.Login;

                if (postNote.CreatingDate == null) {
                    postNote.CreatingDate = DateTime.Now;
                }

                if (postNote.BranchName == null || postNote.BranchName == "") {
                    PrepareNewNote();
                    return;
                }

                bool branchExists = await _context.Notes.AnyAsync(note => note.BranchName == postNote.BranchName);
                if (branchExists) {
                    SetNoteVersion();
                }
                else {
                    postNote.Version = 0;
                }
            }
            void PrepareNewNote() {
                postNote.Version = 0;
                SetUniqueBranchName(postNote.Name);
            }
            void SetUniqueBranchName(string name) {
                string branchName = postNote.Name!;
                while (_context.Notes.FirstOrDefault(note => note.UserName == postNote.UserName && note.BranchName == branchName) != null) {
                    branchName += "s";
                }

                postNote.BranchName = branchName;
            }
            void SetNoteVersion() {
                IQueryable<Note> branch = _context.Notes.Where(note => note.UserName == postNote.UserName && note.BranchName == postNote.BranchName);

                int lastNoteVersion = branch.Max(note => note.Version!.Value);
                if (lastNoteVersion == int.MaxValue) {
                    throw new PrepareNoteException("Db overflow of branch.");
                }

                postNote.Version = lastNoteVersion + 1;
            }
        }

        [Authorize]
        [HttpDelete("Note")]
        public async Task<ActionResult> RemoveBranch(string branchName) {
            string username = HttpContext.GetName();
            IQueryable<Note> targetNotes = _context.Notes.Where(note => note.UserName == username && note.BranchName == branchName);

            bool branchExists = await targetNotes.AnyAsync();
            if (!branchExists) {
                return NotFound();
            }

            await targetNotes.ExecuteDeleteAsync();
            return NoContent();
        }



        private class PrepareNoteException : Exception {
            public PrepareNoteException(string message) : base(message) { }
        }

    }
}
