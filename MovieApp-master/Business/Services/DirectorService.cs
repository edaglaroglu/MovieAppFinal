using Business.Models;
using Business.Models.Results.Base;
using Business.Results;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public interface IDirectorService
    {
        IQueryable<DirectorModel> Query();

        Result Add(DirectorModel model);
        Result Update(DirectorModel model);

        
        //[Obsolete("Do not use this method anymore, use DeleteUser method instead!")]
        Result Delete(int id);

        Result DeleteUser(int id);
    }

    public class DirectorService : IDirectorService
    {
        #region Db Constructor Injection
        private readonly Db _db;

        public DirectorService(Db db)
        {
            _db = db;
        }

        public Result Add(DirectorModel model)
        {
            var nameSqlParameter = new SqlParameter("name", model.Name.Trim());
            var query = _db.Directors.FromSqlRaw("select * from Directors where UPPER(Name) = UPPER(@name)", nameSqlParameter);
            if (query.Any()) 
                return new ErrorResult("Director with the same name already exists!");

            var entity = new Director()
            {
                Name = model.Name.Trim(),
                Surname = model.Surname.Trim(),
                BirthDate = model.BirthDate,
                IsRetired = model.IsRetired
               
            };
            
            _db.Directors.Add(entity);
            _db.SaveChanges();
            return new SuccessResult("Director added successfully.");
        }

        public Result Delete(int id)
        {
            // getting the role entity with relational user entities by role id from the related database table
            var existingEntity = _db.Directors.Include(d => d.Movies).SingleOrDefault(d => d.Id == id);
            if (existingEntity is null)
                return new ErrorResult("Director not found!");

            
            if (existingEntity.Movies.Any())
                return new ErrorResult("Director can't be deleted because it has users!");

            // since there is no relational user entities of the role entity, we can delete it
            _db.Directors.Remove(existingEntity);
            _db.SaveChanges();
            return new SuccessResult("Director deleted successfully.");
        }

        public Result DeleteUser(int id)
        {
            throw new NotImplementedException();
        }

        public IQueryable<DirectorModel> Query()
        {
            return _db.Directors.Include(d => d.Movies).OrderBy(d => d.Name).Select(d => new DirectorModel()
            {
              
              Id = d.Id,   
              Name = d.Name,
              Surname = d.Surname,
              BirthDate = d.BirthDate,
              IsRetired = d.IsRetired,
              NameOutput=d.Name+" "+d.Surname,

                // modified model - entity property assignments for displaying in views
                DateOutput = d.BirthDate.HasValue ? d.BirthDate.Value.ToString("MM/dd/yyyy") : ""
            });
        }

        public Result Update(DirectorModel model)
        {
            var nameSqlParameter = new SqlParameter("name", model.Name.Trim()); 
            var idSqlParameter = new SqlParameter("id", model.Id);
            var surnameSqlParameter = new SqlParameter("surname", model.Surname);

            var query = _db.Directors.FromSqlRaw("select * from Directors where UPPER(Name) = UPPER(@name) and UPPER(Surname) = UPPER(@surname)  and Id != @id", nameSqlParameter, idSqlParameter, surnameSqlParameter);
            if (query.Any()) 
                return new ErrorResult("Director with the same name already exists!");

            var entity = new Director()
            {
                Id = model.Id,
                Name = model.Name.Trim(),
                Surname = model.Surname.Trim(),
                BirthDate = model.BirthDate,
                IsRetired = model.IsRetired
            };

            
            _db.Directors.Update(entity);
            _db.SaveChanges();
            return new SuccessResult("Director updated successfully.");
        }

        #endregion

    }
}