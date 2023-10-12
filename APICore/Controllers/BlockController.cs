using APICore.BasicResponses;
using APICore.Common.DTO.Response;
using APICore.Services;
using APICore.Utils;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace APICore.Controllers
{
    [Route("api/block")]
    public class BlockController : Controller
    {
        private readonly IBlockService _blockService;
        private readonly IMapper _mapper;

        public BlockController(IBlockService blockService, IMapper mapper)
        {
            _blockService = blockService ?? throw new ArgumentNullException(nameof(blockService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("blockUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> BlockUser([Required] int blockedUserId)
        {
            var blockerUserId = this.User.GetUserIdFromToken();
            var result = await _blockService.BlockUserAsync(blockerUserId, blockedUserId);
            return Ok(new ApiOkResponse(result));
        }

        [HttpPost("unblockUser")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> UnblockUser([Required] int blockedUserId)
        {
            var blockerUserId = this.User.GetUserIdFromToken();
            var result = await _blockService.UnblockUserAsync(blockerUserId, blockedUserId);

            return Ok(new ApiOkResponse(result));
        }

        [HttpGet("block-user-list")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<UserResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> UnblockUser()
        {
            var userId = this.User.GetUserIdFromToken();
            var blockedUserList = await _blockService.GetBlockedUserList(userId);
            var mappedUserList = _mapper.Map<List<UserResponse>>(blockedUserList);

            return Ok(new ApiOkResponse(mappedUserList));
        }

    }
}