using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RandomThoughts.Business.ApplicationServices.Comments;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using RandomThoughts.Models.CommentViewModel;
using RandomThoughts.Domain;

namespace RandomThoughts.Controllers.Api
{
    [Produces("application/json")]
    public abstract class CommentApiController : BaseApiController
    {
        private readonly ICommentsApplicationService _commentApplicationService;
        private readonly IMapper _mapper;

        public CommentApiController(IHttpContextAccessor httpContextAccessor, 
            ICommentsApplicationService commentApplicationService,
            IMapper mapper) : base(httpContextAccessor)
        {
            _mapper = mapper;
            _commentApplicationService = commentApplicationService;
        }

        [HttpGet]
        public IEnumerable<CommentsIndexViewModel> GetAllComments(int ParentId)
        {
            var comments = this._commentApplicationService.ReadAll(ParentId);

            var commentsView = _mapper.Map<IEnumerable<Comment>, IEnumerable<CommentsIndexViewModel>>(comments);

            return commentsView;
        }

        [HttpGet("{id}")]
        public IActionResult GetComment(int id)
        {
            var comment = _commentApplicationService.SingleOrDefault(id);

            if (comment != null)
                return Ok(_mapper.Map<Comment, CommentsIndexViewModel>(comment));

            return NotFound(id);
        }

        [HttpPost]
        public IActionResult PostComment([FromBody] CommentsCreateViewModel newComment)
        {
            if (newComment.Body.Length == 0 || newComment.Body.Length > 50)
                return BadRequest();
            newComment.ApplicationUserId = CurrentUserNickName;


            var comment = _mapper.Map<CommentsCreateViewModel, Comment>(newComment);

            comment.Likes = 0;
            comment.ParentDiscriminator = RandomThoughts.Domain.Enums.Discriminator.Thought;
            comment.ApplicationUserId = this.CurrentUserNickName;
            this._commentApplicationService.AddComment(comment);

            try
            {
                _commentApplicationService.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

            return Created("", comment);

        }

        [HttpPut("{id}")]
        public IActionResult PutComment(int id, [FromBody] CommentsEditViewModel commentsEdit)
        {
            if (id != commentsEdit.Id)
            {
                ModelState.AddModelError("Id", "The given Id of the edited model doesn't match with the route Id");
                return BadRequest(ModelState);
            }
            if (!_commentApplicationService.Exists(id))
                return NotFound(id);
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var originalComment = _commentApplicationService.SingleOrDefault(id);
            originalComment.Body = commentsEdit.Body;

            originalComment.ModifiedBy = this.CurrentUserId;

            _commentApplicationService.Update(originalComment);


            try
            {
                _commentApplicationService.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

            return Ok(originalComment);
        }

        [HttpDelete]
        public IActionResult DeleteComment(int id)
        {
            if (!_commentApplicationService.Exists(id))
                return NotFound(id);

            _commentApplicationService.Remove(id);

            try
            {
                _commentApplicationService.SaveChanges();
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }

            return StatusCode(204);
        }
    }
}
