using BusinessObjects.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class AnswerDAO
    {
        private static volatile AnswerDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private AnswerDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static AnswerDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new AnswerDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<Answer> GetAnswerByIdDao(string answerId)
        {
            return await _context.Answers.FindAsync(answerId);
        }

        public async Task<List<Answer>> GetAnswersByQuestionIdDao(string questionId)
        {
            return await _context.Answers
                .Where(a => a.QuestionId == questionId)
                .ToListAsync();
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
