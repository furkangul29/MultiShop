﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MultiShop.Message.DAL.Context;
using MultiShop.Message.DAL.Entities;
using MultiShop.Message.Dtos;

namespace MultiShop.Message.Services
{
    public class UserMessageService : IUserMessageService
    {
        private readonly MessageContext _messageContext;
        private readonly IMapper _mapper;

        public UserMessageService(MessageContext context, IMapper mapper)
        {
            _messageContext = context;
            _mapper = mapper;
        }

        public async Task CreateMessageAsync(CreateMessageDto createMessageDto)
        {
           var values = _mapper.Map<UserMessage>(createMessageDto);
            await _messageContext.UserMessages.AddAsync(values);
            await _messageContext.SaveChangesAsync();

        }

        public  async Task DeleteMessageAsync(int id)
        {
            var values = await _messageContext.UserMessages.FindAsync(id);
            _messageContext.UserMessages.Remove(values);
            await _messageContext.SaveChangesAsync();

        }

        public async Task<List<ResultMessageDto>> GetAllMessageAsync()
        {
            var values = await _messageContext.UserMessages.ToListAsync();
            var result = _mapper.Map<List<ResultMessageDto>>(values);
            return result;

        }

        public  async Task<GetByIdMessageDto> GetByIdMessageAsync(int id)
        {
            var values = await _messageContext.UserMessages.FindAsync(id);
            var result = _mapper.Map<GetByIdMessageDto>(values);
            return result;
        }

        public async Task<List<ResultInboxMessageDto>> GetInboxMessageAsync(string id)
        {
            var values = await _messageContext.UserMessages.Where(x => x.ReceiverId == id).ToListAsync();
            var result = _mapper.Map<List<ResultInboxMessageDto>>(values);
            return result;
        }

        public async Task<List<ResultSendboxMessageDto>> GetSendboxMessageAsync(string id)
        {
            var values = await _messageContext.UserMessages.Where(x => x.SenderId == id).ToListAsync();
            var result = _mapper.Map<List<ResultSendboxMessageDto>>(values);
            return result;
        }

        public async Task<int> GetTotalMessageCount()
        {
            int values = await _messageContext.UserMessages.CountAsync();
            return values;
        }

        public async Task<int> GetTotalMessageCountByReceiverId(string id)
        {
            var values = await _messageContext.UserMessages.Where(x => x.ReceiverId == id).CountAsync();
            return values;
        }

        public async Task UpdateMessageAsync(UpdateMessageDto updateMessageDto)
        {
            var values = _mapper.Map<UserMessage>(updateMessageDto);
            _messageContext.UserMessages.Update(values);
            await _messageContext.SaveChangesAsync();
        }
    }
}