using BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class QuestionDAO
    {
        private static volatile QuestionDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private QuestionDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static QuestionDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new QuestionDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Question> GetQuestionByIdDao(string questionId)
        {
            return await _context.Questions.
                Include(q => q.Answers).
                FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task<List<Question>> GetQuestionsDao()
        {
            return _context.Questions
                .Include(q => q.Answers)
                .ToList();
        }

        public async Task<List<Question>> GetQuestionsByQuizId(string quizid)
        {
            return _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.QuizId == quizid).ToList();
        }

        public async Task<Question> CreateQuestionDao(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<Question> UpdateQuestionDao(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task DeleteQuestionDao(string questionId)
        {
            var question = await GetQuestionByIdDao(questionId);
            if (question == null)
            {
                throw new Exception("Question not found.");
            }

            var answers = _context.Answers.Where(a => a.QuestionId == questionId);
            _context.Answers.RemoveRange(answers);

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

    }
}
