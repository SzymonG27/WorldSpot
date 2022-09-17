using API.Models;
using API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService commentService;
        public CommentController(ICommentService commentService)
        {
            this.commentService = commentService;
        }

        [HttpGet]
        public async Task<IEnumerable<CommentModel>> Get()
        {
            return await commentService.Get();
        }

        [HttpGet("{id}")]
        public async Task<CommentModel> Get(int id)
        {
            return await commentService.Get(id);
        }

        [HttpPost]
        public async Task<ActionResult<CommentModel>> Post([FromBody] CommentModel model)
        {
            var newComment = await commentService.Create(model);
            return CreatedAtAction(nameof(Get), new { id = newComment.Id }, newComment);
        }

        [HttpPut]
        public async Task<ActionResult> Put([FromBody] CommentModel model)
        {
            await commentService.Update(model);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var commentToDelete = await commentService.Get(id);
            if (commentToDelete == null) return BadRequest();
            await commentService.Delete(commentToDelete.Id);
            return NoContent();
        }
    }
}
