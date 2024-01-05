using Business.Models;
using Business.Models.Results.Base;
using Business.Results;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IMovieService
    {
    IQueryable<MovieModel> Query();

    Result Add(MovieModel model);
    Result Update(MovieModel model);

    [Obsolete("Do not use this method anymore, use DeleteUser method instead!")]
    Result Delete(int id);

    Result DeleteUser(int id);
    }

    public class MovieService : IMovieService
    {
    #region Db Constructor Injection
    private readonly Db _db;

    // An object of type Db which inherits from DbContext class is
    // injected to this class through the constructor therefore
    // user CRUD and other operations can be performed with this object.
    public MovieService(Db db)
    {
        _db = db;
    }
     
    public Result Add(MovieModel model)
    {
            List<Movie> existingMovie = _db.Movies.ToList();
            if (existingMovie.Any(m => m.Name.Equals(model.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
                return new ErrorResult("Model with the same user name already exists!");

            
            Movie movieEntity = new Movie()
            {

                Name = model.Name.Trim(),
                Year = model.Year,
                Revenue = model.Revenue,
                DirectorId = model.DirectorId
              
            };

            // adding entity to the related db set
            _db.Movies.Add(movieEntity);

            // changes in all of the db sets are commited to the database with Unit of Work
            _db.SaveChanges();

            return new SuccessResult("Movie is added successfully.");
        }

    public Result Delete(int id)
    {
            var userResourceEntities = _db.MovieGenres.Where(mg => mg.MovieId == id).ToList();

            _db.MovieGenres.RemoveRange(userResourceEntities);

            var userEntity = _db.Movies.SingleOrDefault(m => m.Id == id);

            if (userEntity is null)
                return new ErrorResult("Movie not found!");
            _db.Movies.Remove(userEntity);

            _db.SaveChanges();

            return new SuccessResult("Movie deleted successfully.");
        }

    public Result DeleteUser(int id)
    {
            var movieEntity = _db.Movies.Include(m => m.MovieGenres).SingleOrDefault(m => m.Id == id);
            if (movieEntity is null)
                return new ErrorResult("Movie is not found!");

        
            _db.MovieGenres.RemoveRange(movieEntity.MovieGenres);

            _db.Movies.Remove(movieEntity);

            _db.SaveChanges(); 

            return new SuccessResult("Movie is deleted successfully.");
        }

    public IQueryable<MovieModel> Query()
    {

			return _db.Movies.Include(m => m.Director).OrderByDescending(m => m.Name)
				.ThenBy(m => m.Year)
				.Select(m => new MovieModel()
				{
					// model - entity property assignments
					Id = m.Id,
                    Name = m.Name,
                    Year = m.Year,
                    Revenue = m.Revenue,       
                    DirectorId = m.DirectorId,
                    RevenueOutput=m.Revenue.ToString("N1"),

				});

		}

    public Result Update(MovieModel model)
    {
            var existingMovies = _db.Movies.Where(m => m.Id != model.Id).ToList();

            if (existingMovies.Any(m => m.Name.Equals(model.Name.Trim(), StringComparison.OrdinalIgnoreCase)))
                return new ErrorResult("Movie with the same movie name already exists!");

            var movieEntity = _db.Movies.SingleOrDefault(m => m.Id == model.Id);

            // then checking if the user entity exists
            if (movieEntity is null)
                return new ErrorResult("Movie not found!");

            // then updating the user entity properties
          
            movieEntity.Id = model.Id;
            movieEntity.Name = model.Name.Trim();
            movieEntity.Year = model.Year;
            movieEntity.Revenue = model.Revenue;   
            movieEntity.DirectorId = model.DirectorId ?? 0;
           
            // updating the user entity in the related db set
            _db.Movies.Update(movieEntity);

            // changes in all of the db sets are commited to the database with Unit of Work
            _db.SaveChanges();

            return new SuccessResult("Movie updated successfully.");
        }

         

    #endregion

    }
}
