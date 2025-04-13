using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Application.DTOs;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ClientNexus.Application.Interfaces;

namespace ClientNexus.Application.Services
{
    public class QuestionService : IQuestionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public QuestionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            return _mapper.Map<QuestionResponseDTO>(question);
        }

        public async Task<List<QuestionResponseDTO>> GetQuestionsByClientAsync(int clientId, int offset, int limit)
        {
            if (!await _unitOfWork.Clients.CheckAnyExistsAsync(p => p.Id == clientId))
                throw new KeyNotFoundException("Invalid Client Id.");

            IEnumerable<Question> questions = await _unitOfWork.Questions.GetByConditionAsync(c => c.ClientId == clientId, offset : offset, limit : limit);

            return _mapper.Map<List<QuestionResponseDTO>>(questions);
        }

        public async Task<List<QuestionResponseDTO>> GetQuestionsAnsweredByProviderAsync(int providerId, int offset, int limit)
        {
            if (!await _unitOfWork.ServiceProviders.CheckAnyExistsAsync(p => p.Id == providerId))
                throw new KeyNotFoundException("Invalid Provider Id.");

            IEnumerable<Question> questions = await _unitOfWork.Questions.GetByConditionAsync(c => c.ServiceProviderId == providerId, offset: offset, limit: limit);

            return _mapper.Map<List<QuestionResponseDTO>>(questions);
        }

        public async Task<List<QuestionResponseDTO>> GetAllQuestionsAsync(int offset, int limit, bool onlyUnanswered = false)
        {
            IEnumerable<Question> questions;
            if (onlyUnanswered)
            {
                questions = await _unitOfWork.Questions.GetByConditionAsync(q => q.Status == ServiceStatus.Pending, offset: offset, limit: limit);
            }
            else
            {
                questions = await _unitOfWork.Questions.GetByConditionAsync(q => q.Status == ServiceStatus.Pending || q.Status == ServiceStatus.Done , offset: offset, limit: limit);
            }
            return _mapper.Map<List<QuestionResponseDTO>>(questions);
        }




        }
}
