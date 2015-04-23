using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using taskForPST_LABS.DAL.Models;

namespace taskForPST_LABS.DAL.Repository
{
    public class Repository
    {
        private Entities db = new Entities();

        public List<User> GetUsers()
        {
            return db.Users.ToList();
        }

        public List<User> GetUsers(int startIndex = 0, int count = 0, string sorting = null)
        {
            if (sorting == null && count == 0)
            {
                return GetUsers();
            }
            // Я НЕ ЗНАЮ ПОЧЕМУ НЕЛЬЗЯ параметизировать order by, так что sql injection оставлю тут
            var queryResult = db.Database.SqlQuery<User>(
                    "SELECT * FROM [DBO].[Users] ORDER BY " + sorting).ToList();

            return queryResult.Skip(startIndex).Take(count).ToList();
        }

        public User InsertUser(User user)
        {
            // получаем новый id 
            User p = db.Users.OrderByDescending(c => c.UserId).FirstOrDefault();
            int newId = (null == p ? 0 : p.UserId) + 1;

            user.UserId = newId;
            db.Users.Add(user);
            db.SaveChanges();
            return user;
        }

        public void DeleteUser(User user)
        {
            var delUser = db.Users.First(c => c.UserId == user.UserId);
            db.Users.Remove(delUser);
            db.SaveChanges();
        }

        public void UpdateUser(User user)
        {
            db.Users.Attach(user);
            db.Entry(user).State = EntityState.Modified; ;
            db.SaveChanges();
        }


        public User GetUserByUsername(string username)
        {
            return db.Users.First(u => u.Username == username);
        }
        public User GetUserById(int userId)
        {
            return db.Users.First(u => u.UserId == userId);
        }

        public List<Group> GetGroups()
        {
            return db.Groups.ToList();
        }


        public void UpdateGroup(Group group)
        {
            db.Groups.Attach(group);
            db.Entry(group).State = EntityState.Modified;
            db.SaveChanges();
        }

        public void DeleteGroup(Group group)
        {
            var delGroup = db.Groups.First(c => c.GroupId == group.GroupId);
            db.Groups.Remove(delGroup);
            db.SaveChanges();
        }

        public Group InsertGroup(Group group)
        {
            // получаем новый id 
            Group p = db.Groups.OrderByDescending(c => c.GroupId).FirstOrDefault();
            int newId = (null == p ? 0 : p.GroupId) + 1;

            group.GroupId = newId;
            db.Groups.Add(group);
            db.SaveChanges();
            return group;
        }

        public List<UserGroup> GetUserGroups()
        {
            return db.UserGroups.ToList();
        }

        public void DeleteUserFromGroup(UserGroup userGroup)
        {
            db.UserGroups.Remove(db.UserGroups.First(x => x.Id == userGroup.Id));
            db.SaveChanges();
        }

        public UserGroup AddUserInGroup(int userId, string groupName)
        {
            UserGroup p = db.UserGroups.OrderByDescending(c => c.Id).FirstOrDefault();
            int newId = (null == p ? 0 : p.Id) + 1;
            int groupId = db.Groups.First(c => c.GroupName == groupName).GroupId;
            var userGroup = new UserGroup() { Id = newId, GroupId = groupId, UserId = userId };
            db.UserGroups.Add(userGroup);
            db.SaveChanges();
            return userGroup;
        }
    }
}