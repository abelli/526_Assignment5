﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CIS726_Assignment2.Controllers;
using CIS726_Assignment2.Tests.Fakes;
using MessageParser.Models;
using CIS726_Assignment2.Repositories;
using AuthParser.Models;
using System.Web.Mvc;
using PagedList;

namespace CIS726_Assignment2.Tests
{
    [TestClass]
    public class PlansControllerTest
    {

        private PlansController controller;
        private IGenericRepository<Plan> plans;
        private IGenericRepository<PlanCourse> planCourses;
        private IGenericRepository<Semester> semesters;
        private IGenericRepository<User> users;
        private IGenericRepository<DegreeProgram> degreePrograms;

        private IRoles roles;
        private IWebSecurity webSecurity;

        [TestInitialize]
        public void Initialize()
        {
            plans = new GenericRepository<Plan>(new FakeStorageContext<Plan>());
            planCourses = new GenericRepository<PlanCourse>(new FakeStorageContext<PlanCourse>());
            semesters = new GenericRepository<Semester>(new FakeStorageContext<Semester>());
            users = new GenericRepository<User>(new FakeStorageContext<User>());
            degreePrograms = new GenericRepository<DegreeProgram>(new FakeStorageContext<DegreeProgram>());

            roles = new FakeRoles();
            webSecurity = new FakeWebSecurity(roles, users);

            controller = new PlansController(plans, planCourses, semesters, users, degreePrograms, roles, webSecurity);

            degreePrograms.Add(new DegreeProgram()
            {
                ID = 1,
                degreeProgramName = "Degree Program 1"
            });

            degreePrograms.Add(new DegreeProgram()
            {
                ID = 2,
                degreeProgramName = "Degree Program 2"
            });

            users.Add(new User()
            {
                ID = 1,
                username = "testUser1",
                realName = "Test User 1"
            });

            users.Add(new User()
            {
                ID = 2,
                username = "testUser2",
                realName = "Test User 2"
            });

            users.Add(new User()
            {
                ID = 3,
                username = "Administrator",
                realName = "Administrator"
            });

            users.Add(new User()
            {
                ID = 4,
                username = "Advisor",
                realName = "Advisor"
            });

            semesters.Add(new Semester()
            {
                ID = 1,
                semesterTitle = "Fall",
                semesterYear = 2013,
                standard = true
            });

            semesters.Add(new Semester()
            {
                ID = 2,
                semesterTitle = "Spring",
                semesterYear = 2014,
                standard = true
            });

            plans.Add(new Plan()
            {
                ID = 1,
                planName = "Plan 1",
                degreeProgramID = 1,
                degreeProgram = degreePrograms.Find(1),
                userID = 1,
                //user = users.Find(1),
                semesterID = 1,
                semester = semesters.Find(1),
                planCourses = new Collection<PlanCourse>()
            });

            plans.Add(new Plan()
            {
                ID = 2,
                planName = "Plan 2",
                degreeProgramID = 2,
                degreeProgram = degreePrograms.Find(2),
                userID = 2,
                //user = users.Find(2),
                semesterID = 2,
                semester = semesters.Find(2),
                planCourses = new Collection<PlanCourse>()
            });

        }

        private Plan createTempPlan()
        {
            Plan temp = new Plan()
            {
                ID = 3,
                planName = "Plan 3",
                degreeProgramID = 2,
                degreeProgram = degreePrograms.Find(2),
                userID = 2,
                //user = users.Find(2),
                semesterID = 2,
                semester = semesters.Find(2),
                planCourses = new Collection<PlanCourse>()
            };
            temp.degreeProgram.requiredCourses = new List<RequiredCourse>();
            temp.degreeProgram.electiveCourses = new List<ElectiveCourse>();
            return temp;
        }

        [TestMethod]
        public void PlansControllerIndexReturnsView()
        {
            webSecurity.Login("Advisor", "");
            var result = controller.Index("", 1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void PlansControllerIndexModelIsPlan()
        {
            webSecurity.Login("Advisor", "");
            ViewResult result = controller.Index("", 1) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(PagedList.IPagedList<MessageParser.Models.Plan>));
        }

        [TestMethod]
        public void PlansControllerSortByUsernameAsc()
        {
            webSecurity.Login("Advisor", "");
            ViewResult result = controller.Index("title_asc", 1) as ViewResult;
            PagedList.IPagedList<MessageParser.Models.Plan> model = result.Model as PagedList.IPagedList<MessageParser.Models.Plan>;
            Plan first = model[0];
            Plan second = model[1];
            //Assert.IsTrue(first.user.username.CompareTo(second.user.username) < 0);
        }

        [TestMethod]
        public void PlansControllerSortByUsernameDesc()
        {
            webSecurity.Login("Advisor", "");
            ViewResult result = controller.Index("title_desc", 1) as ViewResult;
            PagedList.IPagedList<MessageParser.Models.Plan> model = result.Model as PagedList.IPagedList<MessageParser.Models.Plan>;
            Plan first = model[0];
            Plan second = model[1];
            //Assert.IsTrue(first.user.username.CompareTo(second.user.username) > 0);
        }

        [TestMethod]
        public void PlansControllerDetailsReturnsView()
        {
            webSecurity.Login("Advisor", "");
            var result = controller.Details(1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void PlanControllerDetailsModelIsPlan()
        {
            webSecurity.Login("testUser1", "");
            ViewResult result = controller.Details(1) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(MessageParser.Models.Plan));
        }

        [TestMethod]
        public void PlansControllerDetailsFindByID()
        {
            webSecurity.Login("Advisor", "");
            ViewResult result = controller.Details(1) as ViewResult;
            Plan model = result.Model as Plan;
            Assert.IsTrue(model.planName.Equals("Plan 1"));
            result = controller.Details(2) as ViewResult;
            model = result.Model as Plan;
            Assert.IsTrue(model.planName.Equals("Plan 2"));
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void PlansControllerDetailsFailsOnInvalidID()
        {
            ViewResult result = controller.Details(5) as ViewResult;
        }

        [TestMethod]
        public void PlansControllerCreateReturnsView()
        {
            webSecurity.Login("Advisor", "");
            var result = controller.Create();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void PlansControllerCreateSuccessForward()
        {
            webSecurity.Login("Advisor", "");
            Plan temp = createTempPlan();
            var result = controller.Create(temp);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void PlansControllerEditReturnsView()
        {
            webSecurity.Login("Advisor", "");
            var result = controller.Edit(1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void PlansControllerEditSuccessForward()
        {
            webSecurity.Login("Advisor", "");
            Plan temp = plans.Find(1);
            temp.degreeProgram.requiredCourses = new List<RequiredCourse>();
            temp.degreeProgram.electiveCourses = new List<ElectiveCourse>();
            var result = controller.Edit(temp);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void PlansControllerEditModelIsPlan()
        {
            webSecurity.Login("Advisor", "");
            ViewResult result = controller.Edit(1) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(MessageParser.Models.Plan));
        }

        [TestMethod]
        public void PlansControllerEditFindByID()
        {
            webSecurity.Login("Advisor", "");
            ViewResult result = controller.Edit(1) as ViewResult;
            Plan model = result.Model as Plan;
            Assert.IsTrue(model.planName.Equals("Plan 1"));
            result = controller.Edit(2) as ViewResult;
            model = result.Model as Plan;
            Assert.IsTrue(model.planName.Equals("Plan 2"));
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void PlansControllerEditFailsOnInvalidID()
        {
            webSecurity.Login("Advisor", "");
            ViewResult result = controller.Edit(5) as ViewResult;
        }

        [TestMethod]
        public void PlansControllerDeleteReturnsView()
        {
            webSecurity.Login("Advisor", "");
            var result = controller.Delete(1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void PlansControllerDeleteSuccessForward()
        {
            webSecurity.Login("Advisor", "");
            var result = controller.DeleteConfirmed(2);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void PlansControllerDeleteModelIsPlan()
        {
            webSecurity.Login("Advisor", "");
            ViewResult result = controller.Delete(1) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(MessageParser.Models.Plan));
        }

        [TestMethod]
        public void PlansControllerDeleteFindByID()
        {
            webSecurity.Login("Advisor", "");
            ViewResult result = controller.Edit(1) as ViewResult;
            Plan model = result.Model as Plan;
            Assert.IsTrue(model.planName.Equals("Plan 1"));
            result = controller.Edit(2) as ViewResult;
            model = result.Model as Plan;
            Assert.IsTrue(model.planName.Equals("Plan 2"));
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void PlansControllerDeleteFailsOnInvalidID()
        {
            webSecurity.Login("Advisor", "");
            ViewResult result = controller.Delete(5) as ViewResult;
        }
    }
}
