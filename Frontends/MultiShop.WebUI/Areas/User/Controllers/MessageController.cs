﻿using Microsoft.AspNetCore.Mvc;
using MultiShop.WebUI.Services.Interfaces;
using MultiShop.WebUI.Services.MessageServices;

namespace MultiShop.WebUI.Areas.User.Controllers
{
    [Area("User")]
    public class MessageController : Controller
    {
        private readonly IUserService _userService;
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        public async Task< IActionResult> Inbox()
        {
            var user = await _userService.GetUserInfo();
            var messages = await _messageService.GetInboxMessageAsync(user.Id);
            return View(messages);
        }

        public async Task<IActionResult> Sendbox()
        {
            var user = await _userService.GetUserInfo();
            var messages = await _messageService.GetSendboxMessageAsync(user.Id);
            return View(messages);
        }
    }
}
