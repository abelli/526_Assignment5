﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MessageParser.Models;
using PagedList;
using CIS726_Assignment2.Repositories;
using MessageParser;

namespace CIS726_Assignment2.Controllers
{
    public class CoursesController : Controller
    {
        private ObjectMessageQueue messagequeue;
        /// <summary>
        /// Constructor used by the web application itself
        /// </summary>
        public CoursesController()
        {
            CourseDBContext context = new CourseDBContext();
            context.Configuration.ProxyCreationEnabled = false;
            messagequeue = new ObjectMessageQueue();
        }

        /// <summary>
        /// Constructor for UnitTesting (allows passing in a fake repository)
        /// </summary>
        /// <param name="fake">A "faked" course repository</param>
        public CoursesController(IGenericRepository<Course> fake, IGenericRepository<PrerequisiteCourse> fakePrereq)
        {
            messagequeue = new ObjectMessageQueue();
        }

        //
        // GET: /Courses/

        /**
         * @russfeld
         * Paging from 
         * http://www.asp.net/mvc/tutorials/getting-started-with-ef-using-mvc/sorting-filtering-and-paging-with-the-entity-framework-in-an-asp-net-mvc-application
         */

        public ActionResult Index(string sortOrder, int? page, string filterString)
        {
            int pageSize = 100;
            int pageNumber = (page ?? 1);

            String currentSort = "";
            String numSort = "";
            String titleSort = "";
            String hoursSort = "";

            String selectedPrefix = "";
            ViewBag.ugrad = true;
            ViewBag.grad = true;
            ViewBag.minHrs = "0";
            ViewBag.maxHrs = "18";
            ViewBag.minNum = "0";
            ViewBag.maxNum = "999";

            bool titleAsc = false;
            bool hoursAsc = false;
            bool numAsc = false;

            List<Course> allCourses = Request<Course>.GetAll("A", "B");

            var courseList = allCourses.AsQueryable();
            

            if (!String.IsNullOrEmpty(filterString))
            {
                ViewBag.filterString = filterString;

                String[] filters = filterString.Split(';');

                //parsing filters
                foreach (string filter in filters)
                {
                    if (filter.Contains("prefix"))
                    {
                        String prefixes = filter.Replace("prefix:", "");
                        courseList = courseList.Where(c => c.coursePrefix.Equals(prefixes));
                        selectedPrefix = prefixes;
                        ViewBag.filtered = true;
                    }

                    if (filter.Equals("ugrad"))
                    {
                        courseList = courseList.Where(c => c.undergrad.Equals(true));
                        ViewBag.grad = false;
                        ViewBag.filtered = true;
                    }
                    if (filter.Equals("grad"))
                    {
                        courseList = courseList.Where(c => c.graduate.Equals(true));
                        ViewBag.ugrad = false;
                        ViewBag.filtered = true;
                    }
                    if (filter.Equals("none"))
                    {
                        courseList = courseList.Where(c => c.courseNumber.Equals(-1));
                        ViewBag.ugrad = false;
                        ViewBag.grad = false;
                        ViewBag.filtered = true;
                    }

                    int minNum = 0;
                    if (filter.Contains("minNum"))
                    {
                        String minNumber = filter.Replace("minNum:", "");
                        if (Int32.TryParse(minNumber, out minNum))
                        {
                            if (minNum > 0)
                            {
                                courseList = courseList.Where(c => c.courseNumber >= minNum);
                                ViewBag.minNum = minNum.ToString();
                                ViewBag.filtered = true;
                            }
                        }
                    }

                    int maxNum = 999;
                    if (filter.Contains("maxNum"))
                    {
                        String maxNumber = filter.Replace("maxNum:", "");
                        if (Int32.TryParse(maxNumber, out maxNum))
                        {
                            if (maxNum < 999)
                            {
                                courseList = courseList.Where(c => c.courseNumber <= maxNum);
                                ViewBag.maxNum = maxNum.ToString();
                                ViewBag.filtered = true;
                            }
                        }
                    }

                    int minHrs = 0;
                    if (filter.Contains("minHrs"))
                    {
                        String minHours = filter.Replace("minHrs:", "");
                        if (Int32.TryParse(minHours, out minHrs))
                        {
                            if (minHrs > 0)
                            {
                                courseList = courseList.Where(c => c.maxHours >= minHrs);
                                ViewBag.minHrs = minHrs.ToString();
                                ViewBag.filtered = true;
                            }
                        }
                    }

                    int maxHrs = 0;
                    if (filter.Contains("maxHrs"))
                    {
                        String maxHours = filter.Replace("maxHrs:", "");
                        if (Int32.TryParse(maxHours, out maxHrs))
                        {
                            if (maxHrs < 18)
                            {
                                courseList = courseList.Where(c => c.minHours <= maxHrs);
                                ViewBag.maxHrs = maxHrs.ToString();
                                ViewBag.filtered = true;
                            }
                        }
                    }
                }
            }
            else
            {
                ViewBag.filterString = "";
                ViewBag.filtered = false;
            }

            //default sort
            if (sortOrder == null)
            {
                sortOrder = "num_asc";
            }

            String[] sorts = sortOrder.Split(';');

            int lastNum = -1;
            int lastTitle = -1;
            int lastHours = -1;

            //parsing sorts
            for (int i = 0; i < sorts.Length; i++)
            {
                if (sorts[i].StartsWith("num"))
                {
                    if (lastNum > 0)
                    {
                        sorts[lastNum] = "";
                    }
                    else
                    {
                        lastNum = i;
                    }
                }
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
                if (sorts[i].StartsWith("hours"))
                {
                    if (lastHours > 0)
                    {
                        sorts[lastHours] = "";
                    }
                    else
                    {
                        lastHours = i;
                    }
                }
            }

            var courseListEnum = courseList.AsEnumerable();

            //doing sorts
            foreach (string s in sorts)
            {
                if (s.Length <= 0)
                {
                    continue;
                }
                currentSort = currentSort + s + ";";
                if (s.Equals("num_asc"))
                {
                    courseListEnum = courseListEnum.OrderBy(x => x.courseCatalogNumber);
                    titleSort = titleSort + s + ";";
                    hoursSort = hoursSort + s + ";";
                    numAsc = true;
                }
                if (s.Equals("num_desc"))
                {
                    courseListEnum = courseListEnum.OrderByDescending(x => x.courseCatalogNumber);
                    titleSort = titleSort + s + ";";
                    hoursSort = hoursSort + s + ";";
                    numAsc = false;
                }
                if (s.Equals("title_asc"))
                {
                    courseListEnum = courseListEnum.OrderBy(x => x.courseTitle);
                    numSort = numSort + s + ";";
                    hoursSort = hoursSort + s + ";";
                    titleAsc = true;
                }
                if (s.Equals("title_desc"))
                {
                    courseListEnum = courseListEnum.OrderByDescending(x => x.courseTitle);
                    numSort = numSort + s + ";";
                    hoursSort = hoursSort + s + ";";
                    titleAsc = false;
                }
                if (s.Equals("hours_asc"))
                {
                    courseListEnum = courseListEnum.OrderBy(x => x.courseHours);
                    numSort = numSort + s + ";";
                    titleSort = titleSort + s + ";";
                    hoursAsc = true;
                }
                if (s.Equals("hours_desc"))
                {
                    courseListEnum = courseListEnum.OrderByDescending(x => x.courseHours);
                    numSort = numSort + s + ";";
                    titleSort = titleSort + s + ";";
                    hoursAsc = false;
                }
            }

            //setting up needed variables in ViewBag
            List<string> prefixList = allCourses.Select(x => x.coursePrefix).Distinct().ToList();
            prefixList.Sort();
            prefixList.Insert(0, "any");
            ViewBag.prefixes  = new SelectList(prefixList, selectedPrefix);

            ViewBag.currentSort = currentSort;
            ViewBag.hoursSort = hoursSort;
            ViewBag.titleSort = titleSort;
            ViewBag.numSort = numSort;

            ViewBag.titleAsc = titleAsc;
            ViewBag.hoursAsc = hoursAsc;
            ViewBag.numAsc = numAsc;

            return View(courseListEnum.ToPagedList(pageNumber, pageSize));
        }

        /// <summary>
        /// Post Action for the Filter Form on the Index page
        /// </summary>
        /// <param name="sortOrder"></param>
        /// <param name="page"></param>
        /// <param name="prefixes"></param>
        /// <param name="ugrad"></param>
        /// <param name="grad"></param>
        /// <param name="minNumber"></param>
        /// <param name="maxNumber"></param>
        /// <param name="minHours"></param>
        /// <param name="maxHours"></param>
        /// <param name="filtered"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string sortOrder, int? page, string prefixes, bool? ugrad, bool? grad, string minNumber, string maxNumber, string minHours, string maxHours, string filtered)
        {
            String filter = "";

            bool filterUgrad = (ugrad ?? true);
            bool filterGrad = (grad ?? true);

            if (!String.IsNullOrEmpty(prefixes) && !prefixes.Contains("any"))
            {
                filter = filter + "prefix:" + prefixes + ";";
            }
            if (!filterGrad && filterUgrad)
            {
                filter = filter + "ugrad;";
            }
            if (!filterUgrad && filterGrad)
            {
                filter = filter + "grad;";
            }
            if (!filterUgrad && !filterGrad)
            {
                filter = filter + "none;";
            }

            int minNum = 0;
            if (!String.IsNullOrEmpty(minNumber))
            {
                if (Int32.TryParse(minNumber, out minNum))
                {
                    if (minNum > 0)
                    {
                        filter = filter + "minNum:" + minNum + ";";
                    }
                }
            }

            int maxNum = 999;
            if (!String.IsNullOrEmpty(maxNumber))
            {
                if (Int32.TryParse(maxNumber, out maxNum))
                {
                    if (maxNum < 999)
                    {
                        filter = filter + "maxNum:" + maxNum + ";";
                    }
                }
            }

            int minHrs = 0;
            if (!String.IsNullOrEmpty(minHours))
            {
                if (Int32.TryParse(minHours, out minHrs))
                {
                    if (minHrs > 0)
                    {
                        filter = filter + "minHrs:" + minHrs + ";";
                    }
                }
            }

            int maxHrs = 0;
            if (!String.IsNullOrEmpty(maxHours))
            {
                if (Int32.TryParse(maxHours, out maxHrs))
                {
                    if (maxHrs < 18)
                    {
                        filter = filter + "maxHrs:" + maxHrs + ";";
                    }
                }
            }

            return RedirectToAction("Index", new { sortOrder = "", page = 1, filterString = filter } );
        }

        //
        // GET: /Courses/Details/5

        public ActionResult Details(int id = 0)
        {
            Course course = Request<Course>.GetItemByID(id, "A", "B");
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // GET: /Courses/Create
        [Authorize(Roles="Administrator")]
        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Courses/Create

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                Request<Course>.Add(course, "A", "B");
                return RedirectToAction("Index");
            }
            return View(course);
        }

        //
        // GET: /Courses/Edit/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(int id = 0)
        {
            Course course = Request<Course>.GetItemByID(id, "A", "B");
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // POST: /Courses/Edit/5

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public ActionResult Edit(Course course, IEnumerable<PrerequisiteCourse> PrerequisiteCourses)
        {
            if (ModelState.IsValid)
            {
                Course cAttached = Request<Course>.GetItemByID(course.ID, "A", "B");
                course.prerequisites = cAttached.prerequisites;

                if (PrerequisiteCourses == null)
                {
                    PrerequisiteCourses = new List<PrerequisiteCourse>();
                }

                if (course.prerequisites == null)
                {
                    course.prerequisites = new List<PrerequisiteCourse>();
                }
                //figure out the courses that were removed from the list
                LinkedList<PrerequisiteCourse> toRemove = new LinkedList<PrerequisiteCourse>();
                foreach (PrerequisiteCourse pcourse in course.prerequisites)
                {
                    if (course.ID > 0)
                    {
                        if (!PrerequisiteCourses.Contains(pcourse))
                        {
                            PrerequisiteCourse elcourseAttached = Request<PrerequisiteCourse>.GetItemByID(pcourse.ID, "A", "B");
                            toRemove.AddFirst(elcourseAttached);
                        }
                    }
                }
                //remove those courses from the database
                while (toRemove.Count > 0)
                {
                    PrerequisiteCourse removeme = toRemove.First();
                    toRemove.RemoveFirst();
                    Request<PrerequisiteCourse>.Delete(removeme.ID, "A", "B");
                }
                //clear the list todo
                course.prerequisites = new List<PrerequisiteCourse>();
                //add the existing courses to the database
                foreach (PrerequisiteCourse pcourse in PrerequisiteCourses)
                {
                    PrerequisiteCourse pcourseAttached = null; ;
                    if (pcourse.ID > 0)
                    {
                        pcourseAttached = Request<PrerequisiteCourse>.GetItemByID(pcourse.ID, "A", "B");
                        Request<PrerequisiteCourse>.Update(pcourseAttached, pcourse, "A", "B");
                    }
                    else
                    {
                        if (Request<Course>.GetItemByID(pcourse.prerequisiteCourseID, "A", "B") != null)
                        {
                            int id = Request<PrerequisiteCourse>.Add(pcourse, "A", "B");
                            pcourseAttached = Request<PrerequisiteCourse>.GetItemByID(id, "A", "B");
                        }
                    }
                    if (pcourseAttached != null)
                    {
                        course.prerequisites.Add(pcourseAttached);
                    }
                }
                Request<Course>.Update(cAttached, course, "A", "B");

                return RedirectToAction("Index");
            }
            if (PrerequisiteCourses != null)
            {
                foreach (PrerequisiteCourse pcourse in PrerequisiteCourses)
                {
                    if (pcourse.prerequisiteCourseID> 0)
                    {
                        pcourse.prerequisiteCourse = Request<Course>.GetItemByID(pcourse.prerequisiteCourseID, "A", "B");
                    }
                }
            }
            course.prerequisites = PrerequisiteCourses.ToList();
            return View(course);
        }


        /// <summary>
        /// @russfeld
        /// This uses AJAX to return the HTML code for a new course row on the form
        /// Mad props to http://ivanz.com/2011/06/16/editing-variable-length-reorderable-collections-in-asp-net-mvc-part-1/
        /// </summary>
        /// <returns></returns>
        public ActionResult PrerequisiteCourseRow(int id = 0)
        {
            if (id > 0)
            {
                PrerequisiteCourse reqcourse = new PrerequisiteCourse()
                {
                    prerequisiteForCourseID = id,
                    prerequisiteCourseID = -1,
                };
                return PartialView("PrerequisiteCourseFormPartial", reqcourse);
            }
            else
            {
                return PartialView("PrerequisiteCourseFormPartial");
            }
        }

        //
        // GET: /Courses/Delete/5
        [Authorize(Roles = "Administrator")]
        public ActionResult Delete(int id = 0)
        {
            Course course = Request<Course>.GetItemByID(id, "A", "B");
            if (course == null)
            {
                return HttpNotFound();
            }
            return View(course);
        }

        //
        // POST: /Courses/Delete/5

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Administrator")]
        public ActionResult DeleteConfirmed(int id)
        {

            bool success = Request<Course>.Delete(id, "A", "B");
            return RedirectToAction("Index");
        }


        /// <summary>
        /// Search function for course auto-complete
        /// </summary>
        /// <param name="term">Term to search for</param>
        /// <returns></returns>
        public JsonResult SearchCourses(string term)
        {
            var keywords = Request<Course>.GetAll("A", "B").AsEnumerable();

            string[] terms = term.Split(' ');
            foreach(string t in terms){
                keywords = keywords.Where(course => course.courseHeader.Contains(t));
            }
            var results = keywords.Select(course => new { course.ID, course.courseHeader });
            return Json(results, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}