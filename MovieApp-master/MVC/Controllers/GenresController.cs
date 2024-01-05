#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataAccess.Contexts;
using DataAccess.Entities;
using Business.Models;
using Business.Services;
using Elfie.Serialization;
using System.ComponentModel.Design;
using Humanizer.Localisation;
using Microsoft.AspNetCore.Authorization;

//Generated from Custom Template.
namespace MVC.Controllers
{
    public class GenresController : Controller
    {
        // TODO: Add service injections here
        private readonly IGenreService _genreService;
        private readonly IMovieService _movieService;


        public GenresController(IGenreService genreService, IMovieService movieService)
        {
            _genreService = genreService;
            _movieService = movieService;   

        }

        // GET: Genres
        public IActionResult Index()
        {
           
            List<GenreModel> genreList = _genreService.Query().ToList(); // TODO: Add get list service logic here
            
            return View(genreList);
        }

        // GET: Genres/Details/5
        public IActionResult Details(int id)
        {
            

            GenreModel genre = _genreService.GetItem(id);
			if (genre == null)
			{
				// instead of returning 404 Not Found HTTP Status Code,
				// we send the message as model to the _Error partial view
				// which is created under ~/Views/Shared folder
				return View("_Error", "genre not found!");
			}
			return View(genre);
		}

		// GET: Genres/Create
		[Authorize(Roles = "admin")]
		public IActionResult Create()
        {
            // TODO: Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
            ViewBag.MovieId = new MultiSelectList(_movieService.Query().ToList(), "Id", "Name");
            return View();
        }

        // POST: Genres/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "admin")]
		public IActionResult Create(GenreModel genre)
        {
            if (ModelState.IsValid)
            {
                var result = _genreService.Add(genre);
                if (result.IsSuccessful)
                {
                    TempData["Message"] = result.Message;
                    return RedirectToAction(nameof(Index));
                }
                ModelState.AddModelError("", result.Message);
            }
            ViewBag.UserId = new MultiSelectList(_movieService.Query().ToList(), "Id", "Name");
            return View(genre);
        }

		// GET: Genres/Edit/5
		[Authorize(Roles = "admin")]
		public IActionResult Edit(int id)
        {


			GenreModel genre = _genreService.GetItem(id);
			if (genre == null)
			{
				return View("_Error", "Genre not found!");
			}
			ViewBag.MovieId = new MultiSelectList(_movieService.Query().ToList(), "Id", "Name");
			return View(genre);


        }

        // POST: Genres/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "admin")]
		public IActionResult Edit(GenreModel genre)
        {
			if (ModelState.IsValid)
			{
				var result = _genreService.Update(genre);
				if (result.IsSuccessful)
				{
					TempData["Message"] = result.Message;
					// Way 1: 
					//return RedirectToAction(nameof(Index));
					// Way 2: redirection with route values
					return RedirectToAction(nameof(Details), new { id = genre.Id });
				}
				ModelState.AddModelError("", result.Message);
			}
			ViewBag.MovieId = new MultiSelectList(_movieService.Query().ToList(), "Id", "Name");
			return View(genre);

		}

		// GET: Genres/Delete/5
		[Authorize(Roles = "admin")]
		public IActionResult Delete(int id)
        {
			var result = _genreService.Delete(id);
			TempData["Message"] = result.Message;
			return RedirectToAction(nameof(Index));
		}

        // POST: Genres/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
		[Authorize(Roles = "admin")]
		public IActionResult DeleteConfirmed(int id)
        {
			var result = _genreService.Delete(id);
			TempData["Message"] = result.Message;
			return RedirectToAction(nameof(Index));
		}
     
	}
}
