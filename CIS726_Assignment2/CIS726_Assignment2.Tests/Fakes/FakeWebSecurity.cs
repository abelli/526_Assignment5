﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CIS726_Assignment2.Repositories;
using MessageParser.Models;
using AuthParser.Models;

namespace CIS726_Assignment2.Tests.Fakes
{
    class FakeWebSecurity : IWebSecurity
    {
        IGenericRepository<User> users;
        IRoles roles;
        int currentUser = 0;

        public FakeWebSecurity(IRoles fakeRole, IGenericRepository<User> aUsers)
        {
            users = aUsers;
            roles = fakeRole;
        }

        public int CurrentUserId
        {
            get{
                if(currentUser == 0){
                    throw new NotSupportedException();
                }else{
                    return currentUser;
                }
            }
        } 

        public string CreateUserAndAccount(string userName, string password, object propertyValues = null, bool requireConfirmationToken = false)
        {
            users.Add(new User()
            {
                ID = users.GetAll().Count(),
                username = userName,
                realName = "Testing Created User"
            });
            return userName;
        }

        public System.Security.Principal.IPrincipal CurrentUser
        {
            get
            {
                if (currentUser == 0)
                {
                    throw new NotSupportedException();
                }
                else
                {
                    return new FakePrincipal(users.Find(currentUser).username, roles);
                }
            }
        }


        public bool Login(string userName, string password, bool persistCookie = false)
        {
            foreach (User user in users.GetAll())
            {
                if (user.username.Equals(userName))
                {
                    currentUser = user.ID;
                    return true;
                }
            }
            return false;
        }

        public void Logout()
        {
            currentUser = 0;
        }
    }
}
