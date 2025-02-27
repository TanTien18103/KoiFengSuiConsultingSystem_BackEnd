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
        private readonly KoiFishPondContext _context;

        public EnrollAnswerDAO(KoiFishPondContext context)
        {
            _context = context;
        }

        public async Task<EnrollAnswer> GetEnrollAnswerById(string enrollAnswerId)
        {
            return await _context.EnrollAnswers.FindAsync(enrollAnswerId);
        }

        public async Task<List<EnrollAnswer>> GetEnrollAnswers()
        {
            return _context.EnrollAnswers.ToList();
        }

        public async Task<EnrollAnswer> CreateEnrollAnswer(EnrollAnswer enrollAnswer)
        {
            _context.EnrollAnswers.Add(enrollAnswer);
            await _context.SaveChangesAsync();
            return enrollAnswer;
        }

        public async Task<EnrollAnswer> UpdateEnrollAnswer(EnrollAnswer enrollAnswer)
        {
            _context.EnrollAnswers.Update(enrollAnswer);
            await _context.SaveChangesAsync();
            return enrollAnswer;
        }

        public async Task DeleteEnrollAnswer(string enrollAnswerId)
        {
            var enrollAnswer = await GetEnrollAnswerById(enrollAnswerId);
            _context.EnrollAnswers.Remove(enrollAnswer);
            await _context.SaveChangesAsync();
        }

    }
}
