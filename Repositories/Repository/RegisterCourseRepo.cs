using BusinessObjects.Models;
using DAOs.DAOs;
using Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repository
{
    public class RegisterCourseRepo : IRegisterCourseRepo
    {
        private readonly RegisterCourseDAO _registerCourseDAO;

        public RegisterCourseRepo(RegisterCourseDAO registerCourseDAO)
        {
            _registerCourseDAO = registerCourseDAO;
        }

        public async Task<RegisterCourse> GetRegisterCourseById(string registerCourseId)
        {
            return await _registerCourseDAO.GetRegisterCourseById(registerCourseId);
        }

        public async Task<RegisterCourse> CreateRegisterCourse(RegisterCourse registerCourse)
        {
            return await _registerCourseDAO.CreateRegisterCourse(registerCourse);
        }

        public async Task<RegisterCourse> UpdateRegisterCourse(RegisterCourse registerCourse)
        {
            return await _registerCourseDAO.UpdateRegisterCourse(registerCourse);
        }

        public async Task DeleteRegisterCourse(string registerCourseId)
        {
            await _registerCourseDAO.DeleteRegisterCourse(registerCourseId);
        }

        public async Task<List<RegisterCourse>> GetRegisterCourses()
        {
            return await _registerCourseDAO.GetRegisterCourses();
        }
    }
}
