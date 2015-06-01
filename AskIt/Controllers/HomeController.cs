using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AskIt.Models;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace AskIt.Controllers
{
    public class HomeController : Controller
    {

        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Index(ChatUser model)
        {
            if (ModelState.IsValid)
            {
                addRoom(model);
            }
            return View(model);
        }

        public ActionResult About()
        {
            return View(getRooms());
        }


        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize]
        public ActionResult Chat()
        {
            return View();
        }

        public List<Room> getRooms()
        {
            using (DataBaseClassDataContext dc = new DataBaseClassDataContext())
            {
                var query =
                    (from q in dc.Room
                     select q).ToList();
                return query;
            }
        }

        #region Help Functions

        private string[] SplitWords(string s)
        {
            return Regex.Split(s, @"\W+");
        }

        private  List<string> getTagsAreas(string inputString)
        {
            string[] wordsArray = SplitWords(inputString);
             List<string> tagsAreas = new List<string>();

            using (DataBaseClassDataContext dbdc = new DataBaseClassDataContext())
            {
                foreach (string word in wordsArray)
                {
                    var tag =
                        (from q in dbdc.Area
                         where q.Tags.Contains(word)
                         select new
                         {
                             q.AreaName
                         }).FirstOrDefault();

                    tagsAreas.Add(tag.ToString());
                }
            }

            return tagsAreas;       
        }

        private string getArea(List<string> tagsAreas)
        {
            string mainArea = tagsAreas.GroupBy(v => v)
            .OrderByDescending(g => g.Count())
            .First()
            .Key;

            return mainArea;
        }

        private void addRoom(ChatUser model)
        {
            using (DataBaseClassDataContext dc = new DataBaseClassDataContext())
            {
                Room room = new Room();
                model.chatGroup = new List<string>(); // initialize collection
                model.chatGroup.Add(model.nameOfGroup);
                var query = (
                    from u in dc.Users
                    from r in dc.Room
                    select new
                    {
                        u.ID
                    }).FirstOrDefault();
                model.authorID = query.ID;
                room.AuthorID = model.authorID;
                room.Name = model.nameOfGroup;
                dc.Room.InsertOnSubmit(room);
                dc.SubmitChanges();
            }
        }

        #endregion
    }
}