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
    [Route("api/report")]
    public class ReportedUsersController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;

        public ReportedUsersController(IReportService reportService, IMapper mapper)
        {
            _reportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("report-user")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> ReportUser([Required] int reportedUserId, string coment)
        {
            var reporterUserId = this.User.GetUserIdFromToken();
            var result = await _reportService.ReportUserAsync(reporterUserId, reportedUserId, coment);
            return Ok(new ApiOkResponse(result));
        }

        [HttpPatch("set-status")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> SetStatus([Required] int reportId, int reportStatus)
        {
            var reporterUserId = this.User.GetUserIdFromToken();
             var result = await _reportService.SetReportStatus(reporterUserId, reportId, reportStatus);
            return Ok(new ApiOkResponse(result));
        }

        [HttpGet("report-user-list")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(List<ReportedUserResponse>), (int)HttpStatusCode.OK)]
        public async Task<IActionResult> GetReportedUserList([FromQuery] ReportUsersFilterRequest filter)
        {
            var reportedUserList = await _reportService.GetReportedUserList(filter);
            var mappedUserList = _mapper.Map<List<ReportedUsers>>(reportedUserList);

            return Ok(new ApiOkResponse(mappedUserList));
        }
    }
}