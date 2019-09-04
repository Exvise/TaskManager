﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ServerAPI.Data;
using ServerAPI.Models;

namespace ServerAPI.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IDBRepository _db;
        

        public UserController(IDBRepository db)
        {
            _db = db;
        }

        [HttpPost("new")]
        public async Task<IActionResult> AddUser([FromBody]User user)
        {
            NewResponseModel newUserResponseModel = new NewResponseModel();
            try
            {
                await _db.AddUser(user);
                newUserResponseModel.Message = "Success !!!";
                newUserResponseModel.CreatedId = user.Id;
                return Ok(newUserResponseModel);
            }
            catch (Exception ex)
            {
                newUserResponseModel.Message = ex.Message;
                return BadRequest(newUserResponseModel);
            }
        }

        [HttpGet("all")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
            => await _db.GetUsers();

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<User>> GetUser(string name, int id)
        {
            if (!string.IsNullOrEmpty(name))
            {
                return await _db.GetUser(name);
            }
            if(id != 0)
            {
               return await _db.GetUser(id);
            }
            return null;
        }

        [Authorize]
        [HttpDelete("{userId}/{projectId}")]
        public async System.Threading.Tasks.Task DeleteUserByProject(int userId, int projectId)
        {
            await _db.DeleteUserFromProject(userId, projectId);
        }

        [Authorize]
        [HttpGet("{projectId}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByProject(int projectId)
            => await _db.GetUsersFromProject(projectId);

    }
}