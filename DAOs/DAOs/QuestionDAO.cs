using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class QuestionDAO
    {
        public static QuestionDAO instance = null;
        private readonly KoiFishPondContext _context;

        public QuestionDAO()
        {
            _context = new KoiFishPondContext();
        }
        public static QuestionDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new QuestionDAO();
                }
                return instance;
            }
        }

        public async Task<Question> GetQuestionByIdDao(string questionId)
        {
            return await _context.Questions.FindAsync(questionId);
        }

        public async Task<List<Question>> GetQuestionsDao()
        {
            return _context.Questions.ToList();
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
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

    }
}
