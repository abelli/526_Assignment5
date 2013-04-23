﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Messaging;
using MessageParser.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using MessageParser.Repositories;
using System.Linq.Expressions;

namespace MessageParser.Processor
{
    public class MessageProcessor
    {
        private GenericRequest request;
        private CourseDBContext context;
        private IGenericRepository<Course> courses;
        private IGenericRepository<DegreeProgram> degreePrograms;
        private IGenericRepository<PrerequisiteCourse> prerequisiteCourses;
        private IGenericRepository<ElectiveCourse> electiveCourses;
        private IGenericRepository<ElectiveList> electiveLists;
        private IGenericRepository<ElectiveListCourse> electiveListCourses;
        private IGenericRepository<Plan> plans;
        private IGenericRepository<PlanCourse> planCourses;
        private IGenericRepository<RequiredCourse> requiredCourses;
        private IGenericRepository<Semester> semesters;

        public MessageProcessor(CourseDBContext context, GenericRequest request)
        {
            this.request = request;
            this.context = context;
            context.Configuration.ProxyCreationEnabled = false;
            courses = new GenericRepository<Course>(new StorageContext<Course>(context));
            degreePrograms = new GenericRepository<DegreeProgram>(new StorageContext<DegreeProgram>(context));
            prerequisiteCourses = new GenericRepository<PrerequisiteCourse>(new StorageContext<PrerequisiteCourse>(context));
            electiveCourses = new GenericRepository<ElectiveCourse>(new StorageContext<ElectiveCourse>(context));
            electiveLists = new GenericRepository<ElectiveList>(new StorageContext<ElectiveList>(context));
            electiveListCourses = new GenericRepository<ElectiveListCourse>(new StorageContext<ElectiveListCourse>(context));
            plans = new GenericRepository<Plan>(new StorageContext<Plan>(context));
            planCourses = new GenericRepository<PlanCourse>(new StorageContext<PlanCourse>(context));
            requiredCourses = new GenericRepository<RequiredCourse>(new StorageContext<RequiredCourse>(context));
            semesters = new GenericRepository<Semester>(new StorageContext<Semester>(context));
            
        }

        public Object GetItemByID()
        {
            int id;
            
            if (request.Type == ModelType.Course)
            {
                id = (request as Request<Course>).RequestedID;
                //var results = context.Courses.Where(t => t.ID == id);
                var search = courses.Where(s => s.ID == id)
                .Include(s => s.degreePrograms.Select(l1 => l1.degreeProgram))
                .Include(s => s.electiveLists.Select(l1 => l1.electiveList));
                
                if (search.Count() > 0)
                {
                    var results = search.First();
                    results.prerequisites = prerequisiteCourses.Where(pr => pr.prerequisiteForCourseID == results.ID)
                     .Include(prf => prf.prerequisiteForCourse).Include(pr => pr.prerequisiteCourse)
                     .ToList();
                    results.prerequisiteFor = prerequisiteCourses.Where(prf => prf.prerequisiteCourseID == results.ID)
                        .Include(prf => prf.prerequisiteForCourse).Include(pr => pr.prerequisiteCourse)
                        .ToList();
                    return (new Response<Course>(request as Request<Course>, results));
                }
                else
                {
                    return new Response<Course>();
                }
            }
            if(request.Type == ModelType.DegreeProgram){
                id = (request as Request<DegreeProgram>).RequestedID;
                var results = degreePrograms.Where(t => t.ID == id);
                if (results.Count() > 0)
                {
                    var output = results
                    .Include(rc => rc.requiredCourses.Select(s => s.course.prerequisites))
                    .Include(rc => rc.requiredCourses.Select(s => s.course.prerequisiteFor))
                    .Include(ec => ec.electiveCourses.Select(s => s.electiveList))
                    .First();
                    return (new Response<DegreeProgram>(request as Request<DegreeProgram>, output));
                }
                else
                {
                    return new Response<DegreeProgram>();
                }
            }
            if(request.Type == ModelType.ElectiveCourse){
                id = (request as Request<ElectiveCourse>).RequestedID;
                var results = electiveCourses.Where(t => t.ID == id);
                if (results.Count() > 0)
                {
                    return (new Response<ElectiveCourse>(request as Request<ElectiveCourse>, results.First()));
                }
                else
                {
                    return new Response<ElectiveCourse>();
                }
            }
            if(request.Type == ModelType.ElectiveList){
                id = (request as Request<ElectiveList>).RequestedID;
                var results = electiveLists.Where(t => t.ID == id);
                if (results.Count() > 0)
                {
                    return (new Response<ElectiveList>(request as Request<ElectiveList>, results.Include(s => s.courses.Select(c => c.course)).First()));
                }
                else
                {
                    return new Response<ElectiveList>();
                }
            }
            if(request.Type == ModelType.ElectiveListCourse){
                id = (request as Request<ElectiveListCourse>).RequestedID;
                var results = electiveListCourses.Where(t => t.ID == id);
                if (results.Count() > 0)
                {
                    return (new Response<ElectiveListCourse>(request as Request<ElectiveListCourse>, results.First()));
                }
                else
                {
                    return new Response<ElectiveListCourse>();
                }
            }
            if(request.Type == ModelType.Plan){
                id = (request as Request<Plan>).RequestedID;
                var results = plans.Where(t => t.ID == id);
                if (results.Count() > 0)
                {
                    return (new Response<Plan>(request as Request<Plan>, results.Include(s => s.degreeProgram)
                    .Include(s => s.semester)
                    .Include(s => s.planCourses.Select(pc => pc.course.prerequisites))
                    .Include(s => s.planCourses.Select(pc => pc.electiveList)).First()));
                }
                else
                {
                    return new Response<Plan>();
                }
            }
            if(request.Type == ModelType.PlanCourse){
                id = (request as Request<PlanCourse>).RequestedID;
                var results = planCourses.Where(t => t.ID == id).Include(t => t.electiveList.courses).Include(t => t.course);
                if (results.Count() > 0)
                {
                    return (new Response<PlanCourse>(request as Request<PlanCourse>, results.First()));
                }
                else
                {
                    return new Response<PlanCourse>();
                }
            }
            if(request.Type == ModelType.PrerequisiteCourse){
                id = (request as Request<PrerequisiteCourse>).RequestedID;
                var results = prerequisiteCourses.Where(t => t.ID == id);
                if (results.Count() > 0)
                {
                    return (new Response<PrerequisiteCourse>(request as Request<PrerequisiteCourse>, results.First()));
                }
                else
                {
                    return new Response<PrerequisiteCourse>();
                }
            }
            if(request.Type == ModelType.RequiredCourse){
                id = (request as Request<RequiredCourse>).RequestedID;
                var results = requiredCourses.Where(t => t.ID == id);
                if (results.Count() > 0)
                {
                    return (new Response<RequiredCourse>(request as Request<RequiredCourse>, results.First()));
                }
                else
                {
                    return new Response<RequiredCourse>();
                }
            }
            if(request.Type == ModelType.Semester){
                id = (request as Request<Semester>).RequestedID;
                var results = semesters.Where(t => t.ID == id);
                if (results.Count() > 0)
                {
                    return (new Response<Semester>(request as Request<Semester>, results.First()));
                }
                else
                {
                    return new Response<Semester>();
                }
            }
            // if(request.Type == ModelType.User){
            //     id = (request as Request<User>).RequestedID;
            //     }
            return null;
        }
        public Object GetAll()
        {
            if (request.Type == ModelType.Course)
            {
                return (new Response<Course>(request as Request<Course>, courses.GetAll().ToList()));
            }
            if (request.Type == ModelType.DegreeProgram)
            {
                return (new Response<DegreeProgram>(request as Request<DegreeProgram>, degreePrograms.GetAll().ToList()));
            }
            if (request.Type == ModelType.ElectiveCourse)
            {
                return (new Response<ElectiveCourse>(request as Request<ElectiveCourse>, electiveCourses.GetAll().ToList()));
            }
            if (request.Type == ModelType.ElectiveList)
            {
                return (new Response<ElectiveList>(request as Request<ElectiveList>, electiveLists.GetAll().ToList()));
            }
            if (request.Type == ModelType.ElectiveListCourse)
            {
                return (new Response<ElectiveListCourse>(request as Request<ElectiveListCourse>, electiveListCourses.GetAll().ToList()));
            }
            if (request.Type == ModelType.Plan)
            {
                return (new Response<Plan>(request as Request<Plan>, plans.GetAll().ToList()));
            }
            if (request.Type == ModelType.PlanCourse)
            {
                return (new Response<PlanCourse>(request as Request<PlanCourse>, planCourses.GetAll().ToList()));
            }
            if (request.Type == ModelType.PrerequisiteCourse)
            {
                return (new Response<PrerequisiteCourse>(request as Request<PrerequisiteCourse>, prerequisiteCourses.GetAll().ToList()));
            }
            if (request.Type == ModelType.RequiredCourse)
            {
                return (new Response<RequiredCourse>(request as Request<RequiredCourse>, requiredCourses.GetAll().ToList()));
            }
            if (request.Type == ModelType.Semester)
            {
                return (new Response<Semester>(request as Request<Semester>, semesters.GetAll().ToList()));
            }
            // if(request.Type == ModelType.User){
            //     id = (request as Request<User>).RequestedID;
            //     }
            return null;
        }

        public Object GetItemsByPlanID()
        {
            if (request.Type == ModelType.PlanCourse)
            {
                return (new Response<PlanCourse>(request as Request<PlanCourse>, planCourses.Where(
                    t => t.planID == (request as Request<PlanCourse>).RequestedID
                    ).ToList()));
            }
            
            return new Response<PlanCourse>();;
        }

        public Object Delete() {
            int id = request.RequestedID;
            try
            {
                if (request.Type == ModelType.Course)
                {
                    return (new Response<Course>(request as Request<Course>, courses.RemoveAndSave(courses.Find(id))));
                }
                if (request.Type == ModelType.DegreeProgram)
                {
                    return (new Response<DegreeProgram>(request as Request<DegreeProgram>, degreePrograms.RemoveAndSave(degreePrograms.Find(id))));
                }
                if (request.Type == ModelType.ElectiveCourse)
                {
                    return (new Response<ElectiveCourse>(request as Request<ElectiveCourse>, electiveCourses.RemoveAndSave(electiveCourses.Find(id))));
                }
                if (request.Type == ModelType.ElectiveList)
                {
                    return (new Response<ElectiveList>(request as Request<ElectiveList>, electiveLists.RemoveAndSave(electiveLists.Find(id))));
                }
                if (request.Type == ModelType.ElectiveListCourse)
                {
                    return (new Response<ElectiveListCourse>(request as Request<ElectiveListCourse>, electiveListCourses.RemoveAndSave(electiveListCourses.Find(id))));
                }
                if (request.Type == ModelType.Plan)
                {
                    return (new Response<Plan>(request as Request<Plan>, plans.RemoveAndSave(plans.Find(id))));
                }
                if (request.Type == ModelType.PlanCourse)
                {
                    return (new Response<PlanCourse>(request as Request<PlanCourse>, planCourses.RemoveAndSave(planCourses.Find(id))));
                }
                if (request.Type == ModelType.PrerequisiteCourse)
                {
                    return (new Response<PrerequisiteCourse>(request as Request<PrerequisiteCourse>, prerequisiteCourses.RemoveAndSave(prerequisiteCourses.Find(id))));
                }
                if (request.Type == ModelType.RequiredCourse)
                {
                    return (new Response<RequiredCourse>(request as Request<RequiredCourse>, requiredCourses.RemoveAndSave(requiredCourses.Find(id))));
                }
                if (request.Type == ModelType.Semester)
                {
                    return (new Response<Semester>(request as Request<Semester>, semesters.RemoveAndSave(semesters.Find(id))));
                }
                // if(request.Type == ModelType.User){
                //     id = (request as Request<User>).RequestedID;
                //     }
            }
            catch
            {
                return null;
            }
            return null;
        }
        public Object Add(){
            try
            {
                if (request.Type == ModelType.Course)
                {
                    Course value = (request as Request<Course>).RequestedMembers.First();
                    return (new Response<Course>(request as Request<Course>, courses.AddAndSave(value)));
                }
                if (request.Type == ModelType.DegreeProgram)
                {
                    DegreeProgram value = (request as Request<DegreeProgram>).RequestedMembers.First();
                    return (new Response<DegreeProgram>(request as Request<DegreeProgram>, degreePrograms.AddAndSave(value)));
                }
                if (request.Type == ModelType.ElectiveCourse)
                {
                    ElectiveCourse value = (request as Request<ElectiveCourse>).RequestedMembers.First();
                    return (new Response<ElectiveCourse>(request as Request<ElectiveCourse>, electiveCourses.AddAndSave(value)));
                }
                if (request.Type == ModelType.ElectiveList)
                {
                    ElectiveList value = (request as Request<ElectiveList>).RequestedMembers.First();
                    return (new Response<ElectiveList>(request as Request<ElectiveList>, electiveLists.AddAndSave(value)));
                }
                if (request.Type == ModelType.ElectiveListCourse)
                {
                    ElectiveListCourse value = (request as Request<ElectiveListCourse>).RequestedMembers.First();
                    return (new Response<ElectiveListCourse>(request as Request<ElectiveListCourse>, electiveListCourses.AddAndSave(value)));
                }
                if (request.Type == ModelType.Plan)
                {
                    Plan value = (request as Request<Plan>).RequestedMembers.First();
                    return (new Response<Plan>(request as Request<Plan>, plans.AddAndSave(value)));
                }
                if (request.Type == ModelType.PlanCourse)
                {
                    PlanCourse value = (request as Request<PlanCourse>).RequestedMembers.First();
                    return (new Response<PlanCourse>(request as Request<PlanCourse>, planCourses.AddAndSave(value)));
                }
                if (request.Type == ModelType.PrerequisiteCourse)
                {
                    PrerequisiteCourse value = (request as Request<PrerequisiteCourse>).RequestedMembers.First();
                    return (new Response<PrerequisiteCourse>(request as Request<PrerequisiteCourse>, prerequisiteCourses.AddAndSave(value)));
                }
                if (request.Type == ModelType.RequiredCourse)
                {
                    RequiredCourse value = (request as Request<RequiredCourse>).RequestedMembers.First();
                    return (new Response<RequiredCourse>(request as Request<RequiredCourse>, requiredCourses.AddAndSave(value)));
                }
                if (request.Type == ModelType.Semester)
                {
                    Semester value = (request as Request<Semester>).RequestedMembers.First();
                    return (new Response<Semester>(request as Request<Semester>, semesters.AddAndSave(value)));
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
        public Object Update()
        {
            try
            {
                if (request.Type == ModelType.Course)
                {
                    Course value = (request as Request<Course>).RequestedMembers[0];
                    Course replace = (request as Request<Course>).RequestedMembers[1];
                    return (new Response<Course>(request as Request<Course>, courses.UpdateAndSave(value, replace)));
                }
                if (request.Type == ModelType.DegreeProgram)
                {
                    DegreeProgram value = (request as Request<DegreeProgram>).RequestedMembers[0];
                    DegreeProgram replace = (request as Request<DegreeProgram>).RequestedMembers[1];
                    return (new Response<DegreeProgram>(request as Request<DegreeProgram>, degreePrograms.UpdateAndSave(value, replace)));
                }
                if (request.Type == ModelType.ElectiveCourse)
                {
                    ElectiveCourse value = (request as Request<ElectiveCourse>).RequestedMembers[0];
                    ElectiveCourse replace = (request as Request<ElectiveCourse>).RequestedMembers[1];
                    return (new Response<ElectiveCourse>(request as Request<ElectiveCourse>, electiveCourses.UpdateAndSave(value, replace)));
                }
                if (request.Type == ModelType.ElectiveList)
                {
                    ElectiveList value = (request as Request<ElectiveList>).RequestedMembers[0];
                    ElectiveList replace = (request as Request<ElectiveList>).RequestedMembers[1];
                    return (new Response<ElectiveList>(request as Request<ElectiveList>, electiveLists.UpdateAndSave(value, replace)));
                }
                if (request.Type == ModelType.ElectiveListCourse)
                {
                    ElectiveListCourse value = (request as Request<ElectiveListCourse>).RequestedMembers[0];
                    ElectiveListCourse replace = (request as Request<ElectiveListCourse>).RequestedMembers[1];
                    return (new Response<ElectiveListCourse>(request as Request<ElectiveListCourse>, electiveListCourses.UpdateAndSave(value, replace)));
                }
                if (request.Type == ModelType.Plan)
                {
                    Plan value = (request as Request<Plan>).RequestedMembers[0];
                    Plan replace = (request as Request<Plan>).RequestedMembers[1];
                    return (new Response<Plan>(request as Request<Plan>, plans.UpdateAndSave(value, replace)));
                }
                if (request.Type == ModelType.PlanCourse)
                {
                    PlanCourse value = (request as Request<PlanCourse>).RequestedMembers[0];
                    PlanCourse replace = (request as Request<PlanCourse>).RequestedMembers[1];
                    return (new Response<PlanCourse>(request as Request<PlanCourse>, planCourses.UpdateAndSave(value, replace)));
                }
                if (request.Type == ModelType.PrerequisiteCourse)
                {
                    PrerequisiteCourse value = (request as Request<PrerequisiteCourse>).RequestedMembers[0];
                    PrerequisiteCourse replace = (request as Request<PrerequisiteCourse>).RequestedMembers[1];
                    return (new Response<PrerequisiteCourse>(request as Request<PrerequisiteCourse>, prerequisiteCourses.UpdateAndSave(value, replace)));
                }
                if (request.Type == ModelType.RequiredCourse)
                {
                    RequiredCourse value = (request as Request<RequiredCourse>).RequestedMembers[0];
                    RequiredCourse replace = (request as Request<RequiredCourse>).RequestedMembers[1];
                    return (new Response<RequiredCourse>(request as Request<RequiredCourse>, requiredCourses.UpdateAndSave(value, replace)));
                }
                if (request.Type == ModelType.Semester)
                {
                    Semester value = (request as Request<Semester>).RequestedMembers[0];
                    Semester replace = (request as Request<Semester>).RequestedMembers[1];
                    return (new Response<Semester>(request as Request<Semester>, semesters.UpdateAndSave(value, replace)));
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
    }

}
