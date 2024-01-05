﻿using Business.Models;
using Business.Models.Results.Base;
using Business.Results;
using DataAccess.Contexts;
using DataAccess.Entities;
using Microsoft.EntityFrameworkCore;

namespace Business;

/// <summary>
/// Performs user CRUD operations.
/// </summary>
public interface IUserService
{
	// method definitions: method definitions must be created here in order to be used in the related controller

	/// <summary>
	/// Queries the records in the Users table.
	/// </summary>
	/// <returns></returns>
	IQueryable<UserModel> Query();

    Result Add(UserModel model);
    Result Update(UserModel model);

	[Obsolete("Do not use this method anymore, use DeleteUser method instead!")]
	Result Delete(int id);

	Result DeleteUser(int id);
}

public class UserService : IUserService // UserService is a IUserService (UserService implements IUserService)
{
    #region Db Constructor Injection
    private readonly Db _db;

    // An object of type Db which inherits from DbContext class is
    // injected to this class through the constructor therefore
    // user CRUD and other operations can be performed with this object.
    public UserService(Db db)
    {
        _db = db;
    }
    #endregion

    // method implementations of the method definitions in the interface
    public IQueryable<UserModel> Query()
    {
        // Query method will be used for generating SQL queries without executing them.
        // From the Users DbSet first order records by IsActive data descending
        // then for records with same IsActive data order UserName ascending
        // then for each element in the User entity collection map user entity
        // properties to the desired user model properties (projection) and return the query.
        // In Entity Framework Core, lazy loading (loading related data automatically without the need to include it) 
        // is not active by default if projection is not used. To use eager loading (loading related data 
        // on-demand with include), you can write the desired related entity property on the DbSet retrieved from 
        // the _db using the Include method either through a lambda expression or a string. If you want to include 
        // the related entity property of the included entity, you should write it through a delegate of type
        // included entity in the ThenInclude method. However, if the ThenInclude method is to be used, 
        // a lambda expression should be used in the Include method.
        return _db.Users.Include(e => e.Role).OrderByDescending(e => e.IsActive)
            .ThenBy(e => e.UserName)
            .Select(e => new UserModel()
            {
                // model - entity property assignments
                Id = e.Id,
                IsActive = e.IsActive,
				Password = e.Password,
                RoleId = e.RoleId,
                Status = e.Status,
                UserName = e.UserName,

                // modified model - entity property assignments for displaying in views
                IsActiveOutput = e.IsActive ? "Yes" : "No",
                RoleNameOutput = e.Role.Name,
                PasswordOutput = new string('*', e.Password.Length) // hidden password value
            });
    }

    public Result Add(UserModel model)
    {
        // Checking if the user with the same user name exists in the database table:
        // Way 1: Data case sensitivity can be simply eliminated by using ToUpper or ToLower methods in both sides,
        // Trim method can be used to remove the white spaces from the beginning and the end of the data.
        //User existingUser = _db.Users.SingleOrDefault(u => u.UserName.ToUpper() == model.UserName.ToUpper().Trim());
        //if (existingUser is not null)
        //    return new ErrorResult("User with the same user name already exists!");
        // Way 2: may cause problems for Turkish characters such as İ, i, I and ı
        //if (_db.Users.Any(u => u.UserName.ToUpper() == model.UserName.ToUpper().Trim()))
        //    return new ErrorResult("User with the same user name already exists!");
        // Way 3: not effective since we are getting all the users from the database table but we can use StringComparison
        // for the correct case insensitive data equality, StringComparison can also be used in Contains, StartsWith and EndsWith methods
        List<User> existingUsers = _db.Users.ToList();
        if (existingUsers.Any(u => u.UserName.Equals(model.UserName.Trim(), StringComparison.OrdinalIgnoreCase)))
            return new ErrorResult("User with the same user name already exists!");

        // entity creation from the model
        User userEntity = new User()
        {
            IsActive = model.IsActive,
            UserName = model.UserName.Trim(),
            Password = model.Password.Trim(),

			// Way 1:
			//RoleId = model.RoleId != null ? model.RoleId.Value : 0,
			// Way 2:
			//RoleId = model.RoleId is not null ? model.RoleId.Value : 0,
			// Way 3:
			//RoleId = model.RoleId.HasValue ? model.RoleId.Value : 0,
			// Way 4:
			//RoleId = model.RoleId.Value, // if we are sure that RoleId has a value
			// Way 5:
			RoleId = model.RoleId ?? 0,

            Status = model.Status
		};
		
		// adding entity to the related db set
        _db.Users.Add(userEntity);
		
		// changes in all of the db sets are commited to the database with Unit of Work
        _db.SaveChanges(); 

        return new SuccessResult("User added successfully.");
    }

    public Result Update(UserModel model)
    {
        // Checking if the user other than the user to be updated (checking by id) with the same user name exists in the database table:
        // Way 1: may cause problems for Turkish characters such as İ, i, I and ı
        //if (_db.Users.Any(u => u.UserName.ToLower() == model.UserName.ToLower().Trim() && u.Id != model.Id))
        //    return new ErrorResult("User with the same user name already exists!");
        // Way 2: not effective since we are getting all the users other than the user with the model id from the database table
        // but we can use StringComparison for the correct case insensitive data equality, StringComparison can also be used in
        // Contains, StartsWith and EndsWith methods
        var existingUsers = _db.Users.Where(u => u.Id != model.Id).ToList();
        if (existingUsers.Any(u => u.UserName.Equals(model.UserName.Trim(), StringComparison.OrdinalIgnoreCase)))
            return new ErrorResult("User with the same user name already exists!");

        // first getting the user entity to be updated from the db set
        var userEntity = _db.Users.SingleOrDefault(u => u.Id == model.Id);

        // then checking if the user entity exists
        if (userEntity is null)
            return new ErrorResult("User not found!");

        // then updating the user entity properties
        userEntity.IsActive = model.IsActive;
        userEntity.UserName = model.UserName.Trim();
        userEntity.Password = model.Password.Trim();
        userEntity.RoleId = model.RoleId ?? 0;
        userEntity.Status = model.Status;

        // updating the user entity in the related db set
        _db.Users.Update(userEntity);

        // changes in all of the db sets are commited to the database with Unit of Work
        _db.SaveChanges();
        
        return new SuccessResult("User updated successfully.");
    }

	// Way 1:
    public Result Delete(int id)
	{
        // for example if we wanted to get the first record of the UserResources DbSet there are 2 ways:
        // Way 1:
        //_db.UserResources.Where(ur => ur.UserId == id).FirstOrDefault();
        // Way 2:
        //_db.UserResources.FirstOrDefault(ur => ur.UserId == id);

		// 1) deleting the relational user resource records:
		// Since there may be none, one or more than one relational user resources, we filter by using Where
		// LINQ method and get an IQueryable (can be thought as a collection) then create the user resource
		// list by calling the ToList LINQ method.
       

        // Way 1: we can iterate through each user resource and delete it from the db set by calling Remove method
        // foreach (var userResourceEntity in userResourceEntities) 
        // {
        //     _db.UserResources.Remove(userResourceEntity);
		//}
        // Way 2: we can use the RemoveRange method to remove one collection from another
       

		// 2) deleting the user record:
        var userEntity = _db.Users.SingleOrDefault(u => u.Id == id);
        if (userEntity is null)
            return new ErrorResult("User not found!");
        _db.Users.Remove(userEntity);

        _db.SaveChanges(); // changes in all of the db sets are commited to the database with Unit of Work

        return new SuccessResult("User deleted successfully.");
	}

	// Way 2: a better way
    public Result DeleteUser(int id)
    {
		// getting the user record joined with the user resources records
        var userEntity = _db.Users.SingleOrDefault(u => u.Id == id);
        if (userEntity is null)
            return new ErrorResult("User not found!");
			
		// 1) deleting the relational user resource records:
        
		
		// 2) deleting the user record:
        _db.Users.Remove(userEntity);
		
        _db.SaveChanges(); // changes in all of the db sets are commited to the database with Unit of Work
		
		return new SuccessResult("User deleted successfully.");
	}
}
