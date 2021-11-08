﻿using Business.Communication;
using Business.Data;
using Business.Domain.Models;
using Business.Domain.Services;
using Business.Extensions;
using Business.Resources;
using Business.Resources.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/v1/account")]
    public class AccountController : DongNguyenController<AccountResource, CreateAccountResource, UpdateAccountResource, Account>
    {
        #region Property
        private readonly IAccountService _accountService;
        #endregion

        #region Constructor
        public AccountController(IAccountService accountService,
            IOptionsMonitor<ResponseMessage> responseMessage) : base(accountService, responseMessage)
        {
            this._accountService = accountService;
        }
        #endregion

        #region Action
        [HttpGet]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<AccountResource>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<AccountResource>>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<AccountResource>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetAllAsync([FromQuery] int page, [FromQuery] int pageSize)
        {
            QueryResource pagintation = new QueryResource(page, pageSize);

            string route = Request.Path.Value;

            var result = await _accountService.ListPaginationAsync(pagintation, route);

            if (!result.Success)
                return BadRequest(result);

            if (result.Resource is null)
                return NoContent();

            return Ok(result);
        }

        [HttpPost]
        [Authorize(Roles = "editor, admin")]
        [ProducesResponseType(typeof(BaseResponse<AccountResource>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<AccountResource>), StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> CreateAsync([FromBody] CreateAccountResource resource)
        {
            if (resource.Role == (int)eRole.Admin)
            {
                if (!User.IsInRole(eRole.Admin.ToDescriptionString()))
                    return BadRequest(new BaseResponse<AccountResource>(ResponseMessage.Values["Account_Not Permitted"]));
            }

            var result = await _accountService.InsertAsync(resource);

            if (!result.Success)
                return BadRequest(result);

            return StatusCode(201, result.Resource);
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(BaseResponse<AccountResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AccountResource>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponse<AccountResource>), StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> GetByIdAsync(int id)
            => await base.GetByIdAsync(id);


        [HttpPut("{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(BaseResponse<AccountResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AccountResource>), StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateAccountResource resource)
            => await base.UpdateAsync(id, resource);


        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(BaseResponse<AccountResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<AccountResource>), StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> DeleteAsync(int id)
            => await base.DeleteAsync(id);
        #endregion
    }
}