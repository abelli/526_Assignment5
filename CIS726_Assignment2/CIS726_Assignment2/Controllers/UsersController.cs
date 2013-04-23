﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using MessageParser.Models;
using AuthParser.Models;
using CIS726_Assignment2.ViewModels;
using CIS726_Assignment2.Repositories;
using PagedList;
using AuthParser;
using MessageParser;

namespace CIS726_Assignment2.Controllers
{
    public class UsersController : Controller
    {
        private IRoles roles;
        private IWebSecurity webSecurity;

        public UsersController()
        {
            CourseDBContext context = new CourseDBContext();
            roles = new RolesImpl();
            webSecurity = new WebSecurityImpl();
        }

        public UsersController(IGenericRepository<User> fake, IRoles fakeRoles, IWebSecurity fakeWebSecurity)
        {
            roles = fakeRoles;
            webSecurity = fakeWebSecurity;
        }

        //
        // GET: /User/

        [Authorize(Roles="Administrator")]
        public ActionResult Index(string sortOrder, int? page)
        {
            int pageSize = 100;
            int pageNumber = (page ?? 1);

            String currentSort = "";

            bool titleAsc = false;

            var usersList = Request<User>.GetAllAuthItems("A", "B").AsEnumerable();

            if (sortOrder == null)
            {
                sortOrder = "title_asc";
            }

            String[] sorts = sortOrder.Split(';');

            int lastTitle = -1;

            for (int i = 0; i < sorts.Length; i++)
            {
                if (sorts[i].StartsWith("title"))
                {
                    if (lastTitle > 0)
                    {
                        sorts[lastTitle] = "";
                    }
                    else
                    {
                        lastTitle = i;
                    }
                }
            }

            foreach (string s in sorts)
            {
                if (s.Length <= 0)
                {
                    continue;
                }
                currentSort = currentSort + s + ";";
                if (s.Equals("title_asc"))
                {
                    usersList = usersList.OrderBy(x => x.username);
                    titleAsc = true;
                }
                if (s.Equals("title_desc"))
                {
                    usersList = usersList.OrderByDescending(x => x.username);
                    titleAsc = false;
                }
            }

            ViewBag.titleAsc = titleAsc;
            ViewBag.currentSort = currentSort;

            return View(usersList.ToPagedList(pageNumber, pageSize));
        }

        //
        // GET: /User/Details/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Details(int id = 0)
        {
            User user = Request<User>.GetAuthItemById(id, "A", "B");
            if (user == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserRoles = roles.GetRolesForUser(user.username).ToList();
            return View(user);
        }

        //
        // GET: /User/Create

        [Authorize(Roles = "Administrator")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /User/Create

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                try
                {
                    webSecurity.CreateUserAndAccount(model.UserName, model.Password, new { realName = model.realName }, false);
                    //WebSecurity.Login(model.UserName, model.Password);
                    return RedirectToAction("Index", "Users");
                }
                catch (MembershipCreateUserException e)
                {
                    ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);

        }

        //
        // GET: /User/Edit/5
        [Authorize]
        public ActionResult Edit(int id = 0)
        {
            UserEdit user = null;
            if (webSecurity.CurrentUser.IsInRole("Administrator") && id > 0)
            {
                user = new UserEdit(Request<User>.GetAuthItemById(id, "A", "B"));
            }
            else
            {
                user = new UserEdit(Request<User>.GetAuthItemByName(User.Identity.Name, "A", "B"));
            }
            if (user == null)
            {
                return HttpNotFound();
            }
            if (webSecurity.CurrentUser.IsInRole("Administrator"))
            {
                ViewBag.AllRoles = roles.GetAllRoles().ToList();
                ViewBag.UserRoles = roles.GetRolesForUser(user.username).ToList();
            }

            return View(user);
        }

        //
        // POST: /User/Edit/5

        [HttpPost]
        [Authorize]
        public ActionResult Edit(UserEdit user, string[] userRoleCheck)
        {
            if (ModelState.IsValid)
            {
                if (webSecurity.CurrentUser.Identity.Name.Equals(user.username) || webSecurity.CurrentUser.IsInRole("Administrator"))
                {
                    User userLink = Request<User>.GetAuthItemById(user.ID, "A", "B");
                    userLink.realName = user.realName;
                    Request<User>.UpdateAuthItem(Request<User>.GetAuthItemById(user.ID, "A", "B"), userLink, "A", "B");
                    

                    if (webSecurity.CurrentUser.IsInRole("Administrator"))
                    {
                        List<String> rolesList = roles.GetAllRoles().ToList();
                        List<string> usersRoles = roles.GetRolesForUser(user.username).ToList();
                        List<string> userNewRoles = userRoleCheck.ToList();
                        foreach (string role in rolesList)
                        {
                            if (usersRoles.Contains(role))
                            {
                                if (!userNewRoles.Contains(role))
                                {
                                    roles.RemoveUserFromRole(user.username, role);
                                }
                            }
                            else
                            {
                                if (userNewRoles.Contains(role))
                                {
                                    roles.AddUserToRole(user.username, role);
                                }
                            }
                        }
                        return RedirectToAction("Index");
                    }
                    return RedirectToAction("Manage", "Accounts");
                }
            }
            user = new UserEdit(Request<User>.GetAuthItemById(user.ID, "A", "B"));
            if (webSecurity.CurrentUser.IsInRole("Administrator"))
            {
                ViewBag.AllRoles = roles.GetAllRoles().ToList();
                ViewBag.UserRoles = roles.GetRolesForUser(user.username).ToList();
            }
            return View(user);
        }

        //
        // GET: /User/Delete/5

        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id = 0)
        {
            User user = Request<User>.GetAuthItemById(id, "A", "B");
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        //
        // POST: /User/Delete/5
        [Authorize(Roles = "Administrator")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            User user = Request<User>.GetAuthItemById(id, "A", "B");
            List<String> rolesList = roles.GetAllRoles().ToList();
            List<string> usersRoles = roles.GetRolesForUser(user.username).ToList();
            foreach (string role in rolesList)
            {
                if (usersRoles.Contains(role))
                {
                    roles.RemoveUserFromRole(user.username, role);
                }
            }
            Request<User>.DeleteAuthItem(user.ID, "A", "B");

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            //users.Dispose();
            base.Dispose(disposing);
        }

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
    }
}