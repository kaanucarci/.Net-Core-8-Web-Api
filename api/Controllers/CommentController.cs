using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using api.Dtos.Comment;
using api.Interfaces;
using api.Mappers;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllesrs
{
    [Route("api/comment")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepo;
        private readonly IStockRepository _stockRepo;
        public CommentController(ICommentRepository commentRepo, IStockRepository stockRepo)
        {
            _commentRepo = commentRepo;
            _stockRepo = stockRepo;
        }   

        [HttpGet]
        
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); 
                

            var comments = await _commentRepo.GetAllAsync();

            var commentDto = comments.Select(s => s.ToCommentDto());

            return Ok(commentDto);
        }


        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); 

            var comment = await _commentRepo.GetByIdAsync(id);

            if (comment == null || !comment.Any())
            {
                return NotFound();
            }

            var commentDto = comment.Select(s => s.ToCommentDto()).ToList();    
            return Ok(commentDto);
        }


        [HttpPost("{stockId:int}")]
        public async Task<IActionResult> Create([FromRoute] int stockId, CreateCommentDto commentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState); 

            if(!await _stockRepo.StockExist(stockId))
            {
                return BadRequest("Stock does not exist!");    
            }

            var commentModel = commentDto.ToCommentFromCreate(stockId);
            await _commentRepo.CreateAsync(commentModel);

            return CreatedAtAction(nameof(GetById), new {id = commentModel.Id}, commentModel.ToCommentDto());
        }

       [HttpPut]
       [Route("{id:int}")]
       public async Task<IActionResult> Update([FromRoute] int id, [FromBody] UpdateCommentDto updateDto)
       {
             if (!ModelState.IsValid)
                return BadRequest(ModelState); 

           var commentModel = await _commentRepo.UpdateByIdAsync(id, updateDto);

           if(commentModel == null)
           {
               return NotFound();
           }

           return Ok(commentModel.ToCommentDto());
       }


       [HttpDelete]
       [Route("{id:int}")]
       public async Task<IActionResult> Delete([FromRoute] int id)
       {
             if (!ModelState.IsValid)
                return BadRequest(ModelState); 
                
           var commentModel = await _commentRepo.DeleteAsync(id);

           if(commentModel == null)
           {
               return NotFound();
           }

           return Ok(commentModel.ToCommentDto());
       }
    }
}