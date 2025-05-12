using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Application.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Models;
using Microsoft.Extensions.Logging;
using ClientNexus.Application.DTO;
using ClientNexus.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ClientNexus.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPushNotification _pushNotification;
        private readonly ILogger<IQuestionService> _logger;

        public QuestionService(IUnitOfWork unitOfWork, IMapper mapper, IPushNotification pushNotification, ILogger<IQuestionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _pushNotification = pushNotification;
            _logger = logger;
        }

        //authorize client
        public async Task<QuestionResponseDTO> CreateQuestionAsync(int clientId, [FromBody]QuestionCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.QuestionBody))
                throw new ArgumentNullException("Question body cannot be empty.");

            if (!await _unitOfWork.Clients.CheckAnyExistsAsync(p => p.Id == clientId))
                throw new KeyNotFoundException("Invalid Client Id.");

            var question = new Question
            {
                QuestionBody = dto.QuestionBody,
                CreatedAt = DateTime.UtcNow,
                ClientId = clientId,
                Status = ServiceStatus.Pending,
                ServiceType = ServiceType.Question
            };

            await _unitOfWork.Questions.AddAsync(question);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<QuestionResponseDTO>(question);
        }

        //authorize provider
        public async Task<QuestionResponseDTO> CreateAnswerAsync(int questionId, int providerId, [FromBody]AnswerCreateDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.AnswerBody))
                throw new ArgumentNullException("Answer body cannot be empty.");

            var question = await _unitOfWork.Questions.GetByIdAsync(questionId) ?? throw new KeyNotFoundException("Invalid question ID.");

            //check that the question was not answered before
            if (question.AnswerBody != null)
                throw new ArgumentException("This question has already been answered.");

            //check if this service provider still exist as active Provider in db
            if (!await _unitOfWork.ServiceProviders.CheckAnyExistsAsync(p => p.Id == providerId))
                throw new KeyNotFoundException("Invalid Provider Id.");

            question.AnswerBody = dto.AnswerBody;
            question.AnsweredAt = DateTime.UtcNow;
            question.ServiceProviderId = providerId;
            question.Status = ServiceStatus.Done;

            await _unitOfWork.SaveChangesAsync();

            //notify the client that his question is answered
            try
            {
                var tokens = await _unitOfWork.Clients.GetByConditionAsync(
                                 c => c.Id == question.ClientId,
                                 c => new NotificationToken { Token = c.NotificationToken! }
             );
                var clientToken = tokens.FirstOrDefault();
                if (clientToken is not null)
                {

                    await _pushNotification.SendNotificationAsync(
                                                                title: "Question Answered",
                                                                body: $"The question you asked at: {question.CreatedAt} has been answered.",
                                                                clientToken.Token);
                    _logger.LogInformation($"The question you asked at:  {question.CreatedAt}  has been answered.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to send notification for answered question,  {ex.Message}");
            }

            return _mapper.Map<QuestionResponseDTO>(question);
        }
        public async Task<QuestionResponseDTO> GetQuestionByIdAsync(int questionId)
        {
            var question = await _unitOfWork.Questions.FirstOrDefaultAsync(q => q.Id == questionId);

            if (question == null)
                throw new KeyNotFoundException("Invalid Question ID.");

            return _mapper.Map<QuestionResponseDTO>(question);
        }

        public async Task<List<QuestionResponseCDTO>> GetQuestionsByClientAsync(int clientId, int offset, int limit, bool onlyUnanswered = false)
        {
            if (!await _unitOfWork.Clients.CheckAnyExistsAsync(c => c.Id == clientId))
                throw new KeyNotFoundException("Invalid Client Id.");
            Expression<Func<Question, bool>> condition;
            if (onlyUnanswered)
            {
                condition = q => q.ClientId == clientId &&  q.Status == ServiceStatus.Pending;
            }
            else
            {
                condition = q => q.ClientId == clientId && (q.Status == ServiceStatus.Pending || q.Status == ServiceStatus.Done);
            }
            var questions = await _unitOfWork.Questions.GetByConditionWithIncludesAsync<QuestionResponseCDTO>(
            condExp: condition,
            includeFunc: c => c
                .Include(q => q.ServiceProvider),

            mapperConfig: _mapper.ConfigurationProvider,
            offset: offset,
            limit: limit
        ); return _mapper.Map<List<QuestionResponseCDTO>>(questions);
        }

        public async Task<List<QuestionResponsePDTO>> GetQuestionsAnsweredByProviderAsync(int providerId, int offset, int limit)
        {
            if (!await _unitOfWork.ServiceProviders.CheckAnyExistsAsync(p => p.Id == providerId))
                throw new KeyNotFoundException("Invalid Provider Id.");

            var questions = await _unitOfWork.Questions.GetByConditionWithIncludesAsync<QuestionResponsePDTO>(
                condExp: q => q.Status == ServiceStatus.Done && q.ServiceProviderId == providerId,
                includeFunc: c => c
                    .Include(q => q.ServiceProvider)
                    .Include(q => q.Client),

                mapperConfig: _mapper.ConfigurationProvider,
                offset: offset,
                limit: limit
            );

            return _mapper.Map<List<QuestionResponsePDTO>>(questions);
        }

        public async Task<List<QuestionResponsePDTO>> GetAllQuestionsAsync(int offset, int limit, bool onlyUnanswered = false)
        {
            Expression<Func<Question, bool>> statusCondition;
            if (onlyUnanswered)
            {
                statusCondition = q => q.Status == ServiceStatus.Pending;
            }
            else
            {
                statusCondition = q => q.Status == ServiceStatus.Pending || q.Status == ServiceStatus.Done;
            }
                var questions = await _unitOfWork.Questions.GetByConditionWithIncludesAsync<QuestionResponsePDTO>(
                condExp: statusCondition,
                includeFunc: c => c
                    .Include(q => q.ServiceProvider)
                    .Include(q => q.Client),

                mapperConfig: _mapper.ConfigurationProvider,
                offset: offset,
                limit: limit
            );

            return _mapper.Map<List<QuestionResponsePDTO>>(questions);
        }

        public async Task DeleteQuestionAsync(int questionId, int currentClientId, UserType role)
        {
            var question = await _unitOfWork.Questions
                .FirstOrDefaultAsync(q => q.Id == questionId);

            if (question == null)
                throw new KeyNotFoundException("Invalid Question ID.");

         
            if (question.ClientId != currentClientId && role != UserType.Admin)
                throw new UnauthorizedAccessException("You are not allowed to delete this question.");

            //if (question.AnswerBody != null)
            //    throw new InvalidOperationException("Cannot delete a question that has already been answered.");

            _unitOfWork.Questions.Delete(question);
            await _unitOfWork.SaveChangesAsync();
        }

        //patch method -> authorize client
        //allow editing only unanswered questions
        public async Task UpdateQuestionAsync(int questionId, int clientId, string updatedBody)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(questionId);

            if (question == null)
                throw new KeyNotFoundException("Invalid Question ID.");

            if (question.ClientId != clientId)
                throw new UnauthorizedAccessException("You are not allowed to edit this question.");

            if (!string.IsNullOrWhiteSpace(question.AnswerBody))
                throw new InvalidOperationException("Cannot edit a question that has already been answered.");

            question.QuestionBody = updatedBody;
            question.UpdatedAt = DateTime.Now;
            await _unitOfWork.SaveChangesAsync();
        }

        //patch method -> authorize client
        public async Task MarkQuestionHelpfulAsync(int questionId, int clientId, bool isHelpful)
        {
            var question = await _unitOfWork.Questions.GetByIdAsync(questionId);

            if (question == null)
                throw new KeyNotFoundException("Invalid Question ID.");

            if (question.ClientId != clientId)
                throw new UnauthorizedAccessException("You are not allowed to rate this question.");

            if (string.IsNullOrWhiteSpace(question.AnswerBody))
                throw new InvalidOperationException("Cannot mark helpfulness on unanswered question.");

            question.IsAnswerHelpful = isHelpful;
            await _unitOfWork.SaveChangesAsync();
        }


    }
}
