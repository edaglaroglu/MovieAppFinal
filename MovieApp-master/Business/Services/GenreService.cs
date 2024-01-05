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
    public interface IGenreService
    {
        IQueryable<GenreModel> Query();
        Result Add(GenreModel model);
        Result Update(GenreModel model);
        Result Delete(int id);
        GenreModel GetItem(int id);

	}
    public class GenreService : IGenreService
    {

        private readonly Db _db;

        public GenreService(Db db)
        {
            _db = db;
        }

        public Result Add(GenreModel model)
        {
            // we want to check a genre with the same name exists

            if( _db.Genres.Any( g => g.Name.ToUpper() == model.Name.ToUpper().Trim()))
            {
                return new ErrorResult("Genre with the same name exists!");
            }

            var entity = new Genre()
            {
            
                Name = model.Name.Trim(),

                // since Title is required in the model, therefore can't be null,
                                            // we don't need to use ?

                // inserting many to many relational entity,
                // ? must be used with UserIdsInput if there is a possibility that it can be null
                MovieGenres = model.MovieIdsInput?.Select(movieId => new MovieGenre()
                {
                    MovieId = movieId

                }).ToList()
            };


            _db.Genres.Add(entity);
            _db.SaveChanges();

            return new SuccessResult("Genre added successfully.");
        }

        public Result Delete(int id)
        {

			var entity = _db.Genres.Include(r => r.MovieGenres).SingleOrDefault(r => r.Id == id);
			if (entity is null)
				return new ErrorResult("Resource not found!");

			// deleting many to many relational entity:
			// deleting relational UserResource entities of the resource entity first
			_db.MovieGenres.RemoveRange(entity.MovieGenres);

			// then deleting the Resource entity
			_db.Genres.Remove(entity);

			_db.SaveChanges();

			return new SuccessResult("Resource deleted successfully.");


		}

        public IQueryable<GenreModel> Query()
        {
            return _db.Genres.Include(g => g.MovieGenres).Select(g => new GenreModel()
            {
                Id = g.Id,
                Name = g.Name,
                // querying over many to many relationship
                MovieNamesOutput = string.Join("<br />", g.MovieGenres.Select(mg => mg.Movie.Name)), // to show user names in details operation
                MovieIdsInput = g.MovieGenres.Select(mg => mg.MovieId).ToList() // to set selected MovieIds in edit operation
            }) .OrderByDescending(g => g.Name);
        }

        public Result Update(GenreModel model)
        {
			if (_db.Genres.Any(g => g.Name.ToUpper() == model.Name.ToUpper().Trim() && g.Id != model.Id) )
			{
				return new ErrorResult("Genre with the same name exists!");
			}

			// deleting many to many relational entity
			var existingEntity = _db.Genres.Include(g => g.MovieGenres).SingleOrDefault(g => g.Id == model.Id);
			if (existingEntity is not null && existingEntity.MovieGenres is not null)
				_db.MovieGenres.RemoveRange(existingEntity.MovieGenres);

			// existingEntity queried from the database must be updated since we got the existingEntity
			// first as above, therefore changes of the existing entity are being tracked by Entity Framework,
			// if disabling of change tracking is required, AsNoTracking method must be used after the DbSet,
			// for example _db.Resources.AsNoTracking()
			existingEntity.Name = model.Name.Trim();
			
			// inserting many to many relational entity
			existingEntity.MovieGenres = model.MovieIdsInput?.Select(movieId => new MovieGenre()
			{
				MovieId = movieId
			}).ToList();

			_db.Genres.Update(existingEntity);
			_db.SaveChanges(); // changes in all DbSets are commited to the database by Unit of Work

			return new SuccessResult("Resource updated successfully.");
		}

		public GenreModel GetItem(int id) => Query().SingleOrDefault(g => g.Id == id);
		
	}
	
}
