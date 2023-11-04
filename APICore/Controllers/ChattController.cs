using APICore.BasicResponses;
using APICore.Common.DTO.Request;
using APICore.Common.DTO.Response;
using APICore.Data.Entities;
using APICore.Services;
using APICore.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace APICore.Controllers
{
    [Authorize]
    [Route("api/chat")]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IMapper _mapper;

        public ChatController(IChatService chatService, IMapper mapper)
        {
            _chatService = chatService ?? throw new ArgumentNullException(nameof(chatService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("send-message")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> CreateMessage([Required] int toId, string msg)
        {
            var fromId = this.User.GetUserIdFromToken();
            var result = await _chatService.CreateChatAsync(fromId, toId, msg);
            return Ok(new ApiOkResponse(result));
        }

        [HttpPost("message-list")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<MessageResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMessageList([FromBody] ChatFilterRequest filter)
        {
            var userId = User.GetUserIdFromToken();
            var messageList = await _chatService.GetChatList(userId, filter);
            var mappedMessageList = _mapper.Map<List<MessageResponse>>(messageList);

            return Ok(new ApiOkResponse(mappedMessageList));
        }
    }
}