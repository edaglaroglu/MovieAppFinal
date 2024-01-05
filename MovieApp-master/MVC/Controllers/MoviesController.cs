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
using Business.Services;
using Business.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using DataAccess.Enums;
using Business.Models.Results.Base;


//Generated from Custom Template.
namespace MVC.Controllers
{
    public class MoviesController : Controller
    {
        // TODO: Add service injections here
        private readonly IMovieService _movieService;
        private readonly IDirectorService _directorService;

        public MoviesController(IMovieService movieService, IDirectorService directorService)
        {
            _movieService = movieService;
            _directorService = directorService;
        }
        // GET: Movies
        [AllowAnonymous]
        public IActionResult GetList()
		{
			// A query is executed and the result is stored in the collection
			// when ToList method is called.
			List<MovieModel> movieList = _movieService.Query().ToList();

			// Way 1: 
			//return View(userList); // model will be passed to the GetList view under Views/Users folder
			// Way 2:
			return View("List", movieList); // model will be passed to the List view under Views/Users folder
		}


	
	

        // GET: Movies/Details/5
        public IActionResult Details(int id)
        {
            MovieModel movie = _movieService.Query().SingleOrDefault(m => m.Id == id); // TODO: Add get item service logic here
			if (movie == null)
            {
                return NotFound();
            }
            return View(movie);
        }

        // GET: Movies/Create
        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            // TODO: Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
           
            
			ViewBag.Directors = new SelectList(_directorService.Query().ToList(), "Id", "NameOutput");

			return View();
		}

        // POST: Movies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult Create(MovieModel movie)
        {
			{
				
				Result result = _movieService.Add(movie); 
				if (result.IsSuccessful)
				{
					
					TempData["Message"] = result.Message; 
					return RedirectToAction(nameof(GetList)); 
				}

		
				ModelState.AddModelError("", result.Message);

			}

			
			return View(movie);
        }

        // GET: Movies/Edit/5
        [Authorize(Roles = "admin")]
        public IActionResult Edit(int id)
        {
            MovieModel movie = _movieService.Query().SingleOrDefault(m => m.Id == id); // getting the model from the service
			if (movie == null)
			{
				return NotFound(); // 404 HTTP Status Code
			}
           
            ViewBag.DirectorId = new SelectList(_directorService.Query().ToList(), "Id", "NameOutput"); 

			return View(movie);
        }

        // POST: Movies/Edit
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult Edit(MovieModel movie)
        {
            if (ModelState.IsValid)
			{
				var result = _movieService.Update(movie); // update the user in the service
				if (result.IsSuccessful)
				{
					// if update operation result is successful, carry successful result message to the List view through the GetList action
					TempData["Message"] = result.Message;
					return RedirectToAction(nameof(GetList));
				}
				ModelState.AddModelError("", result.Message); // if unsuccessful, carry error result message to the view's validation summary
			}
			// Add get related items service logic here to set ViewData if necessary and update null parameter in SelectList with these items
			ViewBag.RoleId = new SelectList(_movieService.Query().ToList(), "Id", "Name");
			return View(movie);
        }

        // GET: Movies/Delete/5
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)

        {
            MovieModel movie = _movieService.Query().SingleOrDefault(m => m.Id == id); // getting the model from the service
			if (movie == null)
			{
				return NotFound();
			}
			return View(movie);
        }

        // POST: Movies/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public IActionResult DeleteConfirmed(int id)
        {
			var result = _movieService.DeleteUser(id);

			// carrying the service result message to the List view through GetList action
			TempData["Message"] = result.Message;

			return RedirectToAction(nameof(GetList));
		}
	}
}
