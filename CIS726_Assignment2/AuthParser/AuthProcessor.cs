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
using MessageParser.Repositories;
using AuthParser.Models;
using System.Linq.Expressions;
using MessageParser;

namespace AuthParser
{
    public class AuthProcessor
    {
        private GenericRequest request;
        private AccountDBContext context;
        private IGenericRepository<User> users;
        private IGenericRepository<Role> roles;
        private IGenericRepository<UserRoles> userroles;

        public AuthProcessor(AccountDBContext context, GenericRequest request)
        {
            this.request = request;
            this.context = context;
            context.Configuration.ProxyCreationEnabled = false;
            users = new GenericRepository<User>(new StorageContext<User>(context));
            roles = new GenericRepository<Role>(new StorageContext<Role>(context));
            userroles = new GenericRepository<UserRoles>(new StorageContext<UserRoles>(context));
        }

        public Object GetItemByID()
        {
            int id;

            if (request.Type == ModelType.User)
            {
                id = (request as Request<User>).RequestedID;
                //var results = context.Courses.Where(t => t.ID == id);
                var search = users.Where(s => s.ID == id);

                if (search.Count() > 0)
                {
                    var results = search.First();
                    return (new Response<User>(request as Request<User>, results));
                }
                else
                {
                    return new Response<User>();
                }
            }
            if (request.Type == ModelType.Role)
            {
                id = (request as Request<Role>).RequestedID;
                //var results = context.Courses.Where(t => t.ID == id);
                var search = roles.Where(s => s.ID == id);

                if (search.Count() > 0)
                {
                    var results = search.First();
                    return (new Response<Role>(request as Request<Role>, results));
                }
                else
                {
                    return new Response<Role>();
                }
            }
            if (request.Type == ModelType.UserRoles)
            {
                id = (request as Request<UserRoles>).RequestedID;
                //var results = context.Courses.Where(t => t.ID == id);
                var search = userroles.Where(s => s.ID == id);

                if (search.Count() > 0)
                {
                    var results = search.First();
                    return (new Response<UserRoles>(request as Request<UserRoles>, results));
                }
                else
                {
                    return new Response<UserRoles>();
                }
            }
            return null;
        }

        public Object GetByName()
        {
            string name = "";
            if (request.Type == ModelType.User)
            {
                name = (request as Request<User>).RequestedName;
                //var results = context.Courses.Where(t => t.ID == id);
                var search = users.Where(s => s.username.Equals(name));

                if (search.Count() > 0)
                {
                    var results = search.First();
                    return (new Response<User>(request as Request<User>, results));
                }
                else
                {
                    return new Response<User>();
                }
            }
            if (request.Type == ModelType.Role)
            {
                name = (request as Request<Role>).RequestedName;
                //var results = context.Courses.Where(t => t.ID == id);
                var search = roles.Where(s => s.rolename.Equals(name));

                if (search.Count() > 0)
                {
                    var results = search.First();
                    return (new Response<Role>(request as Request<Role>, results));
                }
                else
                {
                    return new Response<Role>();
                }
            }
            return null;
        }

        public Object GetAllByName()
        {
            if (request.Type == ModelType.User)
            {
                return (new Response<User>(request as Request<User>, users.GetAll().Where(s => s.username == request.RequestedName).ToList()));
            }
            if (request.Type == ModelType.Role)
            {
                int userid = users.Where(s => s.username == request.RequestedName).Select(s => s.ID).FirstOrDefault();
                List<Role> roles = userroles.Where(s => s.userID == userid).Include(s => s.role).Select(s => s.role).ToList();
                return (new Response<Role>(request as Request<Role>, roles));
            }
            return null;
        }

        public Object GetAll()
        {
            if (request.Type == ModelType.User)
            {
                return (new Response<User>(request as Request<User>, users.GetAll().ToList()));
            }
            if (request.Type == ModelType.Role)
            {
                return (new Response<Role>(request as Request<Role>, roles.GetAll().ToList()));
            }
            if (request.Type == ModelType.UserRoles)
            {
                return (new Response<UserRoles>(request as Request<UserRoles>, userroles.GetAll().ToList()));
            }
            return null;
        }

        public Object Delete()
        {
            int id = request.RequestedID;
            try
            {
                if (request.Type == ModelType.User)
                {
                    return (new Response<User>(request as Request<User>, users.RemoveAndSave(users.Find(id))));
                }
                if (request.Type == ModelType.Role)
                {
                    return (new Response<Role>(request as Request<Role>, roles.RemoveAndSave(roles.Find(id))));
                }
                if (request.Type == ModelType.UserRoles)
                {
                    return (new Response<UserRoles>(request as Request<UserRoles>, userroles.RemoveAndSave(userroles.Find(id))));
                }
            }
            catch
            {
                return null;
            }
            return null;
        }
        public Object Add()
        {
            try
            {
                if (request.Type == ModelType.User)
                {
                    User value = (request as Request<User>).RequestedMembers.First();
                    return (new Response<User>(request as Request<User>, users.AddAndSave(value)));
                }
                if (request.Type == ModelType.Role)
                {
                    Role value = (request as Request<Role>).RequestedMembers.First();
                    return (new Response<Role>(request as Request<Role>, roles.AddAndSave(value)));
                }
                if (request.Type == ModelType.UserRoles)
                {
                    UserRoles value = (request as Request<UserRoles>).RequestedMembers.First();
                    return (new Response<UserRoles>(request as Request<UserRoles>, userroles.AddAndSave(value)));
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
                if (request.Type == ModelType.User)
                {
                    User value = (request as Request<User>).RequestedMembers[0];
                    User replace = (request as Request<User>).RequestedMembers[1];
                    return (new Response<User>(request as Request<User>, users.UpdateAndSave(value, replace)));
                }
                if (request.Type == ModelType.Role)
                {
                    Role value = (request as Request<Role>).RequestedMembers[0];
                    Role replace = (request as Request<Role>).RequestedMembers[1];
                    return (new Response<Role>(request as Request<Role>, roles.UpdateAndSave(value, replace)));
                }
                if (request.Type == ModelType.UserRoles)
                {
                    UserRoles value = (request as Request<UserRoles>).RequestedMembers[0];
                    UserRoles replace = (request as Request<UserRoles>).RequestedMembers[1];
                    return (new Response<UserRoles>(request as Request<UserRoles>, userroles.UpdateAndSave(value, replace)));
                }
            }
            catch
            {
                return null;
            }
            return null;
        }

        public Object GetRolesForUser()
        {
            try
            {
                if (request.Type == ModelType.Role)
                {
                    int userID = (request as Request<Role>).RequestedID;
                    List<Role> roles = userroles.Where(u => u.userID == userID).Select(u => u.role).ToList<Role>();
                    return (new Response<Role>(request as Request<Role>, roles));
                }
                if (request.Type == ModelType.User)
                {
                    int roleID = (request as Request<User>).RequestedID;
                    List<User> users = userroles.Where(u => u.roleID == roleID).Select(u => u.user).ToList<User>();
                    return (new Response<User>(request as Request<User>, users));
                }
                if (request.Type == ModelType.UserRoles)
                {
                    int userID = (request as Request<UserRoles>).RequestedID;
                    List<UserRoles> users = userroles.Where(u => u.userID == userID).Include(u => u.role).ToList<UserRoles>();
                    return (new Response<UserRoles>(request as Request<UserRoles>, users));
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
