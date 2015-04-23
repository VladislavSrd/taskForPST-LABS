using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using taskForPST_LABS.DAL.Models;
using taskForPST_LABS.DAL.Repository;

namespace taskForPST_LABS.Controllers
{
    public class HomeController : Controller
    {
        private Repository _repository = new Repository();
        
        public ActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            ViewBag.FullName = GetCurrentUser().Name + " " + GetCurrentUser().Surname;
            return View();
        }

        [Authorize]
        public ActionResult Table()
        {
            if (User.Identity.IsAuthenticated)
                ViewBag.FullName = GetCurrentUser().Name + " " + GetCurrentUser().Surname;
            return View();
        }

        [HttpPost]
        public JsonResult UserList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null) 
        {
            if (!GetCurrentUser().HaveReadAccess())
            {
                return Json(new { Result = "ERROR", Message = "У вас нет прав на чтение таблицы 'Пользователи'" });
            }
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { Result = "ERROR", Message = "Не авторизированный пользователь" });
            }
            //try
            //{
                var users =
                    _repository.GetUsers(jtStartIndex,jtPageSize,jtSorting)
                        .Select(e => new {e.UserId, e.Name, e.Username, e.Password, e.Surname, e.Lastname, e.Email});
                return Json(new {Result = "OK", Records = users});

            //}
            //catch (Exception ex)
            //{
            //    return Json(new {Result = "ERROR", Message = ex.Message});
            //}
        }

        [HttpPost]
        public JsonResult CreateUser(User record)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { Result = "ERROR", Message = "Не авторизированный пользователь" });
            }
            if (!GetCurrentUser().HaveWriteAccess())
            {
                return Json(new { Result = "ERROR", Message = "У вас нет прав на редактирование таблицы" });
            }
            bool userExists = _repository.GetUsers().Any(user => user.Username == record.Username);
            bool emailExists = _repository.GetUsers().Any(user => user.Email == record.Email);
            if (userExists)
            {
                return Json(new { Result = "ERROR", Message = "Пользователь с таким username существует" });
            }
            if (emailExists)
            {
                return Json(new { Result = "ERROR", Message = "Пользователь с таким email существует" });
            }
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new
                    {
                        Result = "ERROR",
                        Message = "Неверно заполнены поля! "
                    });
                
                }
                
               User addedUser = _repository.InsertUser(record);
               return Json(new { Result = "OK", Record = addedUser });
            }
            catch (Exception ex)
            {
                return Json(new {Result = "ERROR", Message = ex.Message});
            }
        }

        [HttpPost]
        public JsonResult UpdateUser(User record)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return Json(new { Result = "ERROR", Message = "Не авторизированный пользователь" });
                }
                if (!GetCurrentUser().HaveWriteAccess())
                {
                    return Json(new { Result = "ERROR", Message = "У вас нет прав на редактирование таблицы" });
                }
                if (GetCurrentUser().UserId == record.UserId)
                {
                    return Json(new { Result = "ERROR", Message = "Нельзя редактировать свои же данные" });
                }


                _repository.UpdateUser(record);

                return Json(new {Result = "OK"});
            }
            catch (Exception ex)
            {
                return Json(new {Result = "ERROR", Message = ex.Message});
            }
        }

        [HttpPost]
        public JsonResult UserGroupsList(int userId)
        {
            try
            {
                var lol = new Entities();
                var curUser = lol.Users.Find(userId);
                var userGroups =
                    curUser.UserGroups.Select(x => new {x.Id, x.GroupId, x.Group.GroupName, x.Group.Read, x.Group.Write});

                return Json(new {Result = "OK", Records = userGroups});
            }
            catch (Exception ex)
            {
                return Json(new {Result = "ERROR", Message = ex.Message});
            }
        }

        public JsonResult DeleteUser(User record)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { Result = "ERROR", Message = "Не авторизированный пользователь" });
            }
            if (record.HaveWriteAccess())
            {
                return Json(new { Result = "ERROR", Message = "Нельзя удалить пользователя с правами на запись" });
            }
            if (GetCurrentUser().UserId == record.UserId)
            {
                return Json(new { Result = "ERROR", Message = "Нельзя удалять свою же учетную запись" });
            }
            if (!GetCurrentUser().HaveWriteAccess())
            {
                return Json(new { Result = "ERROR", Message = "У вас нет прав на запись/удаление/изменение" });
            }
            try
            {
                _repository.DeleteUser(record);

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }
       
        [HttpPost]
        public JsonResult DeleteUserFromGroup(UserGroup record)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new {Result = "ERROR", Message = "Не авторизированный пользователь"});
            }
            if (!GetCurrentUser().HaveWriteAccess())
            {
                return Json(new {Result = "ERROR", Message = "У вас нет прав на запись/удаление/изменение"});
            }
            if (GetCurrentUser().UserId == record.UserId)
            {
                return Json(new { Result = "ERROR", Message = "Вы не можете удалять себя из группы" });
            }
            try
            {
                _repository.DeleteUserFromGroup(record);
                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new {Result = "ERROR", Message = ex.Message});
            }
        }

        [HttpPost]
        public JsonResult AddUserInGroup(int userId,string GroupName)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { Result = "ERROR", Message = "Не авторизированный пользователь" });
            }
            if (!GetCurrentUser().HaveWriteAccess())
            {
                return Json(new { Result = "ERROR", Message = "У вас нет прав на запись/удаление/изменение" });
            }
            if (GetCurrentUser().UserId == userId)
            {
                return Json(new { Result = "ERROR", Message = "Нельзя изменять свою же учетную запись" });
            
            }
            if (_repository.GetUserById(userId).UserGroups.Any(x => x.Group.GroupName == GroupName))
            {
                return Json(new { Result = "ERROR", Message = "Пользователь уже состоит в данной группе" });
            }
           var addedUserGroup =  _repository.AddUserInGroup(userId,GroupName);
            var userGroup =
                new
                {
                    Id = addedUserGroup.Id,
                    GroupId = addedUserGroup.GroupId,
                    GroupName = addedUserGroup.Group.GroupName,
                    Read = addedUserGroup.Group.Read,
                    Write = addedUserGroup.Group.Write
                };
            return Json(new { Result = "OK", Record = userGroup });
        }


        #region GroupActions
        [HttpPost]
        public JsonResult GroupList()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { Result = "ERROR", Message = "Не авторизированный пользователь" });
            }
            if (!GetCurrentUser().HaveReadAccess())
            {
                return Json(new { Result = "ERROR", Message = "У вас нет прав на чтение таблицы 'Группы'" });
            }
            try
            {
                var groups =
                    _repository.GetGroups()
                        .Select(e => new { e.GroupId, e.GroupName, e.Read, e.Write});
                return Json(new { Result = "OK", Records = groups });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult DeleteGroup(Group record)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { Result = "ERROR", Message = "Не авторизированный пользователь" });
            }
            if (!GetCurrentUser().HaveWriteAccess())
            {
                return Json(new { Result = "ERROR", Message = "У вас нет прав на запись/удаление/изменение" });
            }
            if (_repository.GetUserGroups().Any(c => c.GroupId == record.GroupId))
            {
                return Json(new { Result = "ERROR", Message = "Нельзя удалить группу, в которой есть пользователи" });
            }
           
            try
            {
                _repository.DeleteGroup(record);

                return Json(new { Result = "OK" });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult CreateGroup(Group record)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { Result = "ERROR", Message = "Не авторизированный пользователь" });
            }
            if (!GetCurrentUser().HaveWriteAccess())
            {
                return Json(new { Result = "ERROR", Message = "У вас нет прав на редактирование таблицы" });
            }
            if (GetCurrentUser().UserGroups.Any(x => x.Group.GroupName == record.GroupName))
            {
                return Json(new { Result = "ERROR", Message = "Нельзя меня права группы в которой состоишь" });
            }
            try
            {
                if (record.Write)
                    record.Read = true;
                Group addedGroup = _repository.InsertGroup(record);
                return Json(new { Result = "OK", Record = addedGroup });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateGroup(Group record)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Json(new { Result = "ERROR", Message = "Не авторизированный пользователь" });
            }
            if (!GetCurrentUser().HaveWriteAccess())
            {
                return Json(new { Result = "ERROR", Message = "У вас нет прав на редактирование таблицы" });
            }
            try
            {
                if (record.Write)
                    record.Read = true;
                _repository.UpdateGroup(record);
                return Json(new { Result = "OK", Record = record });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }


        public JsonResult GetGroupNames()
        {
            var groupNames =_repository.GetGroups().Select(x => x.GroupName).ToList();
            return Json(new { Result = "OK", Options = groupNames });
        }
        #endregion 

        #region HelpFunctions

        private User GetCurrentUser()
        {
            return _repository.GetUserByUsername(User.Identity.Name);
        }

        #endregion
    }
}