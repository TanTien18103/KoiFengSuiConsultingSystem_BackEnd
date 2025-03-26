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
            return await _context.Questions
                .Include(q => q.Answers)
                .ToListAsync();
        }

        public async Task<List<Question>> GetQuestionsByQuizId(string quizid)
        {
            return await _context.Questions
                .Include(q => q.Answers)
                .Where(q => q.QuizId == quizid).ToListAsync();
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

            var answerIds = _context.Answers
                .Where(a => a.QuestionId == questionId)
                .Select(a => a.AnswerId)
                .ToList();

            if (answerIds.Any())
            {
                var enrollAnswers = _context.EnrollAnswers.Where(ea => answerIds.Contains(ea.AnswerId)).ToList();
                foreach (var enroll in enrollAnswers)
                {
                    enroll.AnswerId = null;
                    enroll.Correct = false;
                }
                await _context.SaveChangesAsync(); 

                var answers = _context.Answers.Where(a => a.QuestionId == questionId);
                _context.Answers.RemoveRange(answers);
            }

            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }



    }
}
