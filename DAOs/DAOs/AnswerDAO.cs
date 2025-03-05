using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class AnswerDAO
    {
        public static AnswerDAO instance = null;
        private readonly KoiFishPondContext _context;

        public AnswerDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static AnswerDAO Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AnswerDAO();
                }
                return instance;
            }
        }

        public async Task<Answer> GetAnswerByIdDao(string answerId)
        {
            return await _context.Answers.FindAsync(answerId);
        }

        public async Task<List<Answer>> GetAnswersByQuestionIdDao(string questionId)
        {
            return _context.Answers.Where(a => a.QuestionId == questionId).ToList();
        }

        public async Task<Answer> CreateAnswerDao(Answer answer)
        {
            _context.Answers.Add(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task<Answer> UpdateAnswerDao(Answer answer)
        {
            _context.Answers.Update(answer);
            await _context.SaveChangesAsync();
            return answer;
        }

        public async Task DeleteAnswerDao(string answerId)
        {
            var answer = await GetAnswerByIdDao(answerId);
            _context.Answers.Remove(answer);
            await _context.SaveChangesAsync();
        }

    }
}
