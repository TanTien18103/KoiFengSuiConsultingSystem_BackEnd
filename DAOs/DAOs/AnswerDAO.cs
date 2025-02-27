using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class AnswerDAO
    {
        private readonly KoiFishPondContext _context;

        public AnswerDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<Answer> GetAnswerById(string answerId)
        {
            return await _context.Answers.FindAsync(answerId);
        }

        public async Task<List<Answer>> GetAnswersByQuestionId(string questionId)
        {
            return _context.Answers.Where(a => a.QuestionId == questionId).ToList();
        }

        public async Task<Answer> CreateAnswer(Answer answer)
        {
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task<Answer> UpdateAnswer(Answer answer)
        {
            _context.Answers.Update(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task DeleteAnswer(string answerId)
        {
            var answer = await GetAnswerById(answerId);
            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
        }

    }
}
