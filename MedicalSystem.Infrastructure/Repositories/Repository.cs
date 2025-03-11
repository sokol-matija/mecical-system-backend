using Microsoft.EntityFrameworkCore;
using MedicalSystem.Core.Interfaces;
using System.Linq.Expressions;
using MedicalSystem.Infrastructure.Data;

namespace MedicalSystem.Infrastructure.Repositories
{
	public class Repository<T> : IRepository<T> where T : class
	{
		protected readonly MedicalSystemDbContext _context;
		protected readonly DbSet<T> _dbSet;

		public Repository(MedicalSystemDbContext context)
		{
			_context = context;
			_dbSet = context.Set<T>();
		}

		public virtual async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public virtual async Task<T> GetByIdAsync(int id)
		{
			return await _dbSet.FindAsync(id);
		}

		public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
		{
			return await _dbSet.Where(predicate).ToListAsync();
		}

		public virtual async Task<T> AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
			await _context.SaveChangesAsync();
			return entity;
		}

		public virtual async Task UpdateAsync(T entity)
		{
			// Get the ID property using reflection
			var idProperty = typeof(T).GetProperty("Id");
			if (idProperty == null)
				throw new InvalidOperationException("Entity does not have an Id property.");
			
			var id = (int)idProperty.GetValue(entity);

			// Get the existing entity
			var existingEntity = await _dbSet.FindAsync(id);
			if (existingEntity == null)
				throw new KeyNotFoundException($"Entity with ID {id} not found.");

			// Detach the existing entity to avoid tracking conflicts
			_context.Entry(existingEntity).State = EntityState.Detached;
			
			// Attach the new entity and mark it as modified
			_context.Entry(entity).State = EntityState.Modified;

			try
			{
				// Save changes
				await _context.SaveChangesAsync();
			}
			catch (DbUpdateConcurrencyException ex)
			{
				// Handle concurrency issues
				throw new InvalidOperationException("The record you attempted to edit was modified by another user. Please refresh and try again.", ex);
			}
			catch (DbUpdateException ex)
			{
				// Handle database update errors
				throw new InvalidOperationException($"An error occurred while updating the entity: {ex.InnerException?.Message ?? ex.Message}", ex);
			}
		}

		public virtual async Task DeleteAsync(T entity)
		{
			_dbSet.Remove(entity);
			await _context.SaveChangesAsync();
		}

		public virtual async Task<bool> ExistsAsync(int id)
		{
			return await _dbSet.FindAsync(id) != null;
		}

		public virtual async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null)
		{
			if (predicate == null)
				return await _dbSet.CountAsync();
			return await _dbSet.CountAsync(predicate);
		}
	}
}