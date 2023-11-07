using APICore.BasicResponses;
using APICore.Common.DTO.Request;
using APICore.Common.DTO.Response;
using APICore.Data.Entities;
using APICore.Services;
using APICore.Services.Impls;
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
        public async Task<IActionResult> CreateMessage([Required] int toId, int chatId, string msg)
        {
            var fromId = this.User.GetUserIdFromToken();
            var result = await _chatService.CreateChatAsync(fromId, toId, chatId, msg);
            return Ok(new ApiOkResponse(result));
        }

        [HttpGet("message-list")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<int>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetChatList()
        {
            var userId = User.GetUserIdFromToken();
            var chatList = await _chatService.GetChatList(userId);
            var chatResponse = _mapper.Map<List<ChatResponse>>(chatList);

            return Ok(new ApiOkResponse(chatResponse));
        }

        [HttpDelete("delete-message")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> DeleteMessage([Required] int messageId)
        {
            var userId = this.User.GetUserIdFromToken();
            var result = await _chatService.DeleteMessage(userId, messageId);
            return Ok(new ApiOkResponse(result));
        }

        [HttpGet("get-messages-list")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(List<MessageResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetMessagesList([Required] int chatId, int? page, int? perPage)
        {
            var userId = User.GetUserIdFromToken();
            int pag = page ?? 1;
            int perPag = perPage ?? 10;
            var messages = await _chatService.GetMessageList(userId, chatId, pag, perPag);
            var messageResponse = _mapper.Map<List<MessageResponse>>(messages.ToList());
            Response.AddPagingHeaders(messages.GetPaginationData);
            return Ok(new ApiOkResponse(messageResponse));
        }
        [HttpGet("users-list")]
        [ProducesResponseType(typeof(List<UserResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> GetUserList(string name, int? page, int? perPage)
        {
            var userId = User.GetUserIdFromToken();
            int pag = page ?? 1;
            int perPag = perPage ?? 10;
            var users = await _chatService.GetUserList(userId, name, pag, perPag);
            var userResponse = _mapper.Map<List<UserResponse>>(users.ToList());
            Response.AddPagingHeaders(users.GetPaginationData);
            return Ok(new ApiOkResponse(userResponse));
        }

    }
}