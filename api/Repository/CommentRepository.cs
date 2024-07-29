using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Data;
using api.Dtos.Comment;
using api.Interfaces;
using api.Models;
using Microsoft.EntityFrameworkCore;

namespace api.Repository
{
    public class CommentRepository : ICommentRepository
    {
        private readonly ApplicationDbContext _context;
        public CommentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Comment?> CreateAsync(Comment commentModal)
        {
            await _context.Comments.AddAsync(commentModal);
            await _context.SaveChangesAsync();
            return commentModal;
        }

        public async Task<List<Comment>> GetAllAsync()
        {
            return await _context.Comments.ToListAsync();
        }

        public async Task<List<Comment>> GetByIdAsync(int id)
        {
            return await _context.Comments.Where(c => c.StockId == id).Include(c => c.Stock).ToListAsync();
        }

        public async Task<Comment?> UpdateByIdAsync(int id, UpdateCommentDto commentDto)
        {
            var existingComment = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if(existingComment == null)
            {
                return null;
            }

            existingComment.Title = commentDto.Title;
            existingComment.Content = commentDto.Content;

            await _context.SaveChangesAsync();
            return existingComment;
        }

        public async Task<Comment?> DeleteAsync(int id)
        {
            var commentModel = await _context.Comments.FirstOrDefaultAsync(x => x.Id == id);

            if(commentModel == null)
            {
                return null;
            }  

            _context.Comments.Remove(commentModel);
            await _context.SaveChangesAsync();
            return commentModel;
        }
    }
}