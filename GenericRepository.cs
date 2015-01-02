using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Data.Objects;
using System.Data.Entity.Core.EntityClient;

namespace GenericRepository
{
    public class GenericRepository<TEntity, TContext> : IGenericRepository<TEntity> 
        where TEntity : class 
        where TContext : ObjectContext, new()
    {
        /// <summary>
        /// The context object for the database
        /// </summary>
        private TContext context = null;

        /// <summary>
        /// The IObjectSet that represents the current entity.
        /// </summary>
        private ObjectSet<TEntity> objectSet;

        /// <summary>
        /// Initializes a new instance of the DataRepository class
        /// </summary>
        public GenericRepository()
        {
            if (context == null)
                context = (TContext)Activator.CreateInstance(typeof(TContext));

            objectSet = context.CreateObjectSet<TEntity>();
        }

        /// <summary>
        /// Gets all records as an IQueryable
        /// </summary>
        /// <returns>An IQueryable object containing the results of the query</returns>
        public IQueryable<TEntity> Fetch()
        {
            return objectSet;
        }

        /// <summary>
        /// Gets all records as an IEnumberable
        /// </summary>
        /// <returns>An IEnumberable object containing the results of the query</returns>
        public IEnumerable<TEntity> GetAll()
        {
            return Fetch().AsEnumerable();
        }

        /// <summary>
        /// Finds a record with the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A collection containing the results of the query</returns>
        public IEnumerable<TEntity> Find(Func<TEntity, bool> predicate)
        {
            return objectSet.Where<TEntity>(predicate);
        }

        /// <summary>
        /// Gets a single record by the specified criteria (usually the unique identifier)
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record that matches the specified criteria</returns>
        public TEntity Single(Func<TEntity, bool> predicate)
        {
            return objectSet.SingleOrDefault<TEntity>(predicate);
        }

        /// <summary>
        /// The first record matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        /// <returns>A single record containing the first record matching the specified criteria</returns>
        public TEntity First(Func<TEntity, bool> predicate)
        {
            return objectSet.FirstOrDefault<TEntity>(predicate);
        }

        /// <summary>
        /// Deletes the specified entitiy
        /// </summary>
        /// <param name="entity">Entity to delete</param>
        /// <exception cref="ArgumentNullException"> if <paramref name="entity"/> is null</exception>
        public void Delete(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            objectSet.DeleteObject(entity);

            SaveChanges(SaveOptions.DetectChangesBeforeSave);
        }

        /// <summary>
        /// Deletes records matching the specified criteria
        /// </summary>
        /// <param name="predicate">Criteria to match on</param>
        public void Delete(Func<TEntity, bool> predicate)
        {
            IEnumerable<TEntity> records = from x in objectSet.Where<TEntity>(predicate) select x;

            foreach (TEntity record in records)
            {
                objectSet.DeleteObject(record);
            }

            SaveChanges(SaveOptions.DetectChangesBeforeSave);
        }

        /// <summary>
        /// Adds the specified entity
        /// </summary>
        /// <param name="entity">Entity to add</param>
        /// <param name="saveAction">User may exclude the save of all the child elements.</param>
        /// <exception cref="ArgumentNullException"> if <paramref name="entity"/> is null</exception>
        public void Add(TEntity entity, bool saveAction = true)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }           

            objectSet.AddObject(entity);

            if(saveAction)
                SaveChanges(SaveOptions.DetectChangesBeforeSave);
        }

        /// <summary>
        /// Adds a list of the specified entities
        /// </summary>
        /// <param name="list">Entities to add</param>
        /// <exception cref="ArgumentNullException"> if <paramref name="entity"/> is null</exception>
        public void Add(List<TEntity> list)
        {
            if (list.Count == 0)
            {
                throw new ArgumentNullException("list of entities");
            }

            foreach (var item in list)
            {
                objectSet.AddObject(item);
            }            

            SaveChanges(SaveOptions.DetectChangesBeforeSave);
        }

        /// <summary>
        /// Updates the specified entity
        /// </summary>
        /// <param name="entity">Entity to update</param>
        /// <param name="predicate">Search criteria to get the entity</param>
        /// <exception cref="ArgumentNullException"> if <paramref name="entity"/> is null</exception>
        public void Update(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException("entity");
            }

            if (context.ObjectStateManager.GetObjectStateEntry(entity).State == System.Data.EntityState.Detached)
            {
                throw new InvalidOperationException("Cannot update a not existing entity");
            }

            SaveChanges(SaveOptions.DetectChangesBeforeSave);
        }

        /// <summary>
        /// Attaches the specified entity
        /// </summary>
        /// <param name="entity">Entity to attach</param>
        public void Attach(TEntity entity)
        {
            objectSet.Attach(entity);
        }

        /// <summary>
        /// Saves all context changes
        /// </summary>
        public void SaveChanges()
        {
            context.SaveChanges();
        }

        /// <summary>
        /// Saves all context changes with the specified SaveOptions
        /// </summary>
        /// <param name="options">Options for saving the context</param>
        public void SaveChanges(SaveOptions options)
        {
            context.SaveChanges(options);
        }

        /// <summary>
        /// Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases all resources used by the WarrantManagement.DataExtract.Dal.ReportDataBase
        /// </summary>
        /// <param name="disposing">A boolean value indicating whether or not to dispose managed resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (context != null)
                {
                    context.Dispose();
                    context = null;
                }
            }
        }
    }
}