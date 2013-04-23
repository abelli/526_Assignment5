﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CIS726_Assignment2.Controllers;
using CIS726_Assignment2.Tests.Fakes;
using MessageParser.Models;
using CIS726_Assignment2.ViewModels;
using CIS726_Assignment2.Repositories;
using AuthParser.Models;
using System.Web.Mvc;
using PagedList;

namespace CIS726_Assignment2.Tests
{
    [TestClass]
    public class UsersControllerTest
    {

        private UsersController controller;
        private IGenericRepository<User> users;

        private IRoles roles;
        private IWebSecurity webSecurity;
       
        [TestInitialize]
        public void Initialize()
        {
            users = new GenericRepository<User>(new FakeStorageContext<User>());

            roles = new FakeRoles();
            webSecurity = new FakeWebSecurity(roles, users);

            controller = new UsersController(users, roles, webSecurity);

            users.Add(new User()
            {
                ID = 1,
                username = "Administrator",
                realName = "Administrator"
            });

            users.Add(new User()
            {
                ID = 2,
                username = "Advisor",
                realName = "Advisor"
            });
        }

        private User createTempUser()
        {
            User user = new User()
            {
                ID = 3,
                username = "Test3",
                realName = "Test User 3"
            };
            return user;
        }


        private RegisterModel createTempRegister()
        {
            RegisterModel model = new RegisterModel(){
                UserName = "Test4",
                Password = "abc123",
                ConfirmPassword = "abc123",
                realName = "Test User 4"
            };
            return model;
        }

        [TestMethod]
        public void UsersControllerIndexReturnsView()
        {
            var result = controller.Index("", 1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void UsersControllerIndexModelIsUser()
        {
            ViewResult result = controller.Index("", 1) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(PagedList.IPagedList<AuthParser.Models.User>));
        }

        [TestMethod]
        public void UsersControllerSortByUsernameAsc()
        {
            ViewResult result = controller.Index("title_asc", 1) as ViewResult;
            PagedList.IPagedList<AuthParser.Models.User> model = result.Model as PagedList.IPagedList<AuthParser.Models.User>;
            User first = model[0];
            User second = model[1];
            Assert.IsTrue(first.username.CompareTo(second.username) < 0);
        }

        [TestMethod]
        public void UsersControllerSortByUsernameTitleDesc()
        {
            ViewResult result = controller.Index("title_desc", 1) as ViewResult;
            PagedList.IPagedList<AuthParser.Models.User> model = result.Model as PagedList.IPagedList<AuthParser.Models.User>;
            User first = model[0];
            User second = model[1];
            Assert.IsTrue(first.username.CompareTo(second.username) > 0);
        }

        [TestMethod]
        public void UsersControllerDetailsReturnsView()
        {
            var result = controller.Details(1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void UserControllerDetailsModelIsUser()
        {
            webSecurity.Login("Administrator", "");
            ViewResult result = controller.Details(1) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(AuthParser.Models.User));
        }

        [TestMethod]
        public void UsersControllerDetailsFindByID()
        {
            ViewResult result = controller.Details(1) as ViewResult;
            User model = result.Model as User;
            Assert.IsTrue(model.username.Equals("Administrator"));
            result = controller.Details(2) as ViewResult;
            model = result.Model as User;
            Assert.IsTrue(model.username.Equals("Advisor"));
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void UsersControllerDetailsFailsOnInvalidID()
        {
            ViewResult result = controller.Details(5) as ViewResult;
        }

        [TestMethod]
        public void UsersControllerCreateReturnsView()
        {
            var result = controller.Create();
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void UsersControllerCreateSuccessForward()
        {
            RegisterModel temp = createTempRegister();
            var result = controller.Create(temp);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void UsersControllerEditReturnsView()
        {
            webSecurity.Login("Administrator", "");
            var result = controller.Edit(1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void UsersControllerEditSuccessForward()
        {
            webSecurity.Login("Administrator", "");
            User temp = users.Find(1);
            var result = controller.Edit(new UserEdit(temp), new string[0]);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void UsersControllerEditModelIsUser()
        {
            webSecurity.Login("Administrator", "");
            ViewResult result = controller.Edit(1) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(CIS726_Assignment2.ViewModels.UserEdit));
        }

        [TestMethod]
        public void UsersControllerEditFindByID()
        {
            webSecurity.Login("Administrator", "");
            ViewResult result = controller.Edit(1) as ViewResult;
            User model = result.Model as User;
            Assert.IsTrue(model.username.Equals("Administrator"));
            result = controller.Edit(2) as ViewResult;
            model = result.Model as User;
            Assert.IsTrue(model.username.Equals("Advisor"));
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void UsersControllerEditFailsOnInvalidID()
        {
            webSecurity.Login("Administrator", "");
            ViewResult result = controller.Edit(5) as ViewResult;
        }

        [TestMethod]
        public void UsersControllerDeleteReturnsView()
        {
            var result = controller.Delete(1);
            Assert.IsInstanceOfType(result, typeof(ViewResult));
        }

        [TestMethod]
        public void UsersControllerDeleteSuccessForward()
        {
            var result = controller.DeleteConfirmed(2);
            Assert.IsInstanceOfType(result, typeof(RedirectToRouteResult));
        }

        [TestMethod]
        public void UsersControllerDeleteModelIsUser()
        {
            ViewResult result = controller.Delete(1) as ViewResult;
            Assert.IsInstanceOfType(result.Model, typeof(AuthParser.Models.User));
        }

        [TestMethod]
        public void UsersControllerDeleteFindByID()
        {
            webSecurity.Login("Administrator", "");
            ViewResult result = controller.Edit(1) as ViewResult;
            User model = result.Model as User;
            Assert.IsTrue(model.username.Equals("Administrator"));
            result = controller.Edit(2) as ViewResult;
            model = result.Model as User;
            Assert.IsTrue(model.username.Equals("Advisor"));
        }

        [TestMethod]
        [ExpectedException(typeof(System.InvalidOperationException))]
        public void UsersControllerDeleteFailsOnInvalidID()
        {
            ViewResult result = controller.Delete(5) as ViewResult;
        }
    }
}

