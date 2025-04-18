﻿using BusinessObjects.Models;
using DAOs.DAOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Repositories.RegisterCourseRepository
{
    public class RegisterCourseRepo : IRegisterCourseRepo
    {
        public Task<RegisterCourse> GetRegisterCourseById(string registerCourseId)
        {
            return RegisterCourseDAO.Instance.GetRegisterCourseByIdDao(registerCourseId);
        }
        public Task<List<RegisterCourse>> GetRegisterCourses()
        {
            return RegisterCourseDAO.Instance.GetRegisterCoursesDao();
        }
        public Task<RegisterCourse> CreateRegisterCourse(RegisterCourse registerCourse)
        {
            return RegisterCourseDAO.Instance.CreateRegisterCourseDao(registerCourse);
        }
        public Task<RegisterCourse> UpdateRegisterCourse(RegisterCourse registerCourse)
        {
            return RegisterCourseDAO.Instance.UpdateRegisterCourseDao(registerCourse);
        }
        public Task DeleteRegisterCourse(string registerCourseId)
        {
            return RegisterCourseDAO.Instance.DeleteRegisterCourseDao(registerCourseId);
        }

        public Task<RegisterCourse> GetRegisterCourseByCourseIdAndCustomerId(string courseId, string customerid)
        {
            return RegisterCourseDAO.Instance.GetRegisterCourseByCourseIdAndCustomerIdDao(courseId, customerid);
        }
        
        public Task<RegisterCourse> UpdateRegisterCourseRating(string enrollCourseId, decimal rating)
        {
            return RegisterCourseDAO.Instance.UpdateRegisterCourseRatingDao(enrollCourseId, rating);
        }
        
        public Task<List<RegisterCourse>> GetRegisterCoursesByCourseId(string courseId)
        {
            return RegisterCourseDAO.Instance.GetRegisterCoursesByCourseIdDao(courseId);
        }

        public Task<RegisterCourse> GetRegisterCourseByEnrollQuizId(string enrollQuizId)
        {
            return RegisterCourseDAO.Instance.GetRegisterCourseByEnrollQuizIdDao(enrollQuizId);
        }
    }
}
