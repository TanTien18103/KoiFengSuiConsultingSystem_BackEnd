using BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAOs.DAOs
{
    public class EnrollAnswerDAO
    {
        private static volatile EnrollAnswerDAO _instance;
        private static readonly object _lock = new object();
        private readonly KoiFishPondContext _context;

        private EnrollAnswerDAO()
        {
            _context = new KoiFishPondContext();
        }

        public static EnrollAnswerDAO Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null)
                        {
                            _instance = new EnrollAnswerDAO();
                        }
                    }
                }
                return _instance;
            }
        }

        public async Task<EnrollAnswer> GetEnrollAnswerByIdDao(string enrollAnswerId)
        {
            return await _context.EnrollAnswers.FindAsync(enrollAnswerId);
        }

        public async Task<List<EnrollAnswer>> GetEnrollAnswersDao()
        {
            return _context.EnrollAnswers.ToList();
        }

        public async Task<EnrollAnswer> CreateEnrollAnswerDao(EnrollAnswer enrollAnswer)
        {
            _context.EnrollAnswers.Add(enrollAnswer);
            await _context.SaveChangesAsync();
            return enrollAnswer;
        }

        public async Task<EnrollAnswer> UpdateEnrollAnswerDao(EnrollAnswer enrollAnswer)
        {
            _context.EnrollAnswers.Update(enrollAnswer);
            await _context.SaveChangesAsync();
            return enrollAnswer;
        }

        public async Task DeleteEnrollAnswerDao(string enrollAnswerId)
        {
            var enrollAnswer = await GetEnrollAnswerByIdDao(enrollAnswerId);
            _context.EnrollAnswers.Remove(enrollAnswer);
            await _context.SaveChangesAsync();
        }
    }
}
