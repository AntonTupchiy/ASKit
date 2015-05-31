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

        [HttpPost]
        public ActionResult Index(ChatUser model)
        {
            if (ModelState.IsValid)
            {
                string inputString = model.inputGroups.ToString();
                model.chatGroup.Add(inputString);
  
            }

            return View(model);
        }


        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
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

        #endregion
    }
}