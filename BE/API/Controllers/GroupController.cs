﻿using AutoMapper;
using Business.Communication;
using Business.Domain.Models;
using Business.Domain.Repositories;
using Business.Domain.Services;
using Business.Extensions;
using Business.Resources;
using Business.Resources.Group;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Serilog;

namespace API.Controllers
{
    [Route("api/v1/group")]
    public class GroupController : DongNguyenController<GroupResource, CreateGroupResource, UpdateGroupResource, Group>
    {
        #region Constructor
        public GroupController(IGroupService groupService,
            IGroupRepository groupRepository,
            IMapper mapper,
            IOptionsMonitor<ResponseMessage> responseMessage) : base(groupService, mapper, responseMessage)
        {
            this._groupRepository = groupRepository;
        }
        #endregion

        #region Property
        private readonly IGroupRepository _groupRepository;
        #endregion

        #region Action
        [HttpGet("search")]
        [Authorize(Roles = "editor, admin")]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<GroupResource>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<GroupResource>>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponse<IEnumerable<GroupResource>>), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> FindAsync([FromQuery] string filterName)
        {
            Log.Information($"{User.Identity?.Name}: find group data with {filterName}-keyword.");

            var result = await _groupRepository.FindByNameAsync(filterName.RemoveSpaceCharacter());

            if (result is null)
                return NoContent();

            return Ok(new BaseResponse<IEnumerable<GroupResource>>(Mapper.Map<IEnumerable<Group>, IEnumerable<GroupResource>>(result)));
        }

        [HttpGet("{id:int}")]
        [Authorize(Roles = "editor, admin")]
        [ProducesResponseType(typeof(BaseResponse<GroupResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GroupResource>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(BaseResponse<GroupResource>), StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> GetByIdAsync(int id)
        {
            Log.Information($"{User.Identity?.Name}: get a group with Id is {id}.");

            return await base.GetByIdAsync(id);
        }

        [HttpPost]
        [Authorize(Roles = "editor, admin")]
        [ProducesResponseType(typeof(BaseResponse<GroupResource>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(BaseResponse<GroupResource>), StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> CreateAsync([FromBody] CreateGroupResource resource)
        {
            Log.Information($"{User.Identity?.Name}: create a group.");

            return await base.CreateAsync(resource);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = "editor, admin")]
        [ProducesResponseType(typeof(BaseResponse<GroupResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GroupResource>), StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> UpdateAsync(int id, [FromBody] UpdateGroupResource resource)
        {
            Log.Information($"{User.Identity?.Name}: update a group with Id is {id}.");

            return await base.UpdateAsync(id, resource);
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = "admin")]
        [ProducesResponseType(typeof(BaseResponse<GroupResource>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(BaseResponse<GroupResource>), StatusCodes.Status400BadRequest)]
        public new async Task<IActionResult> DeleteAsync(int id)
        {
            Log.Information($"{User.Identity?.Name}: delete a group with Id is {id}.");

            return await base.DeleteAsync(id);
        }
        #endregion
    }
}
