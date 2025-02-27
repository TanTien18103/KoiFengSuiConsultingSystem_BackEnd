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
        private readonly KoiFishPondContext _context;

        public QuestionDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Question> GetQuestionById(string questionId)
        {
            return await _context.Questions.FindAsync(questionId);
        }

        public async Task<List<Question>> GetQuestions()
        {
            return _context.Questions.ToList();
        }

        public async Task<Question> CreateQuestion(Question question)
        {
            _context.Questions.Add(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<Question> UpdateQuestion(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task DeleteQuestion(string questionId)
        {
            var question = await GetQuestionById(questionId);
            _context.Questions.Remove(question);
            await _context.SaveChangesAsync();
        }

    }
}
