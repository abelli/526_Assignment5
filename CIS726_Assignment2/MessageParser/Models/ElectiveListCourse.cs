﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace MessageParser.Models
{

    /// <summary>
    /// This class is the many-to-many link between the Elective List class and the courses
    /// that are part of that elective list. It does not contain any other important
    /// information about the link, but is included for simplicity in dealing with the database.
    /// </summary>
    public class ElectiveListCourse : IModel
    {
        [ScaffoldColumn(false)]
        public override int ID { get; set; }

        [Required(ErrorMessage = "A Course ID is required")]
        public int courseID { get; set; }

        [Required(ErrorMessage = "An Elective List ID is required")]
        public int electiveListID { get; set; }

        public virtual Course course { get; set; }
        public virtual ElectiveList electiveList { get; set; }

        public override bool Equals(object obj)
        {
            ElectiveListCourse course = obj as ElectiveListCourse;
            if (course != null)
            {
                if (course.ID == ID)
                {
                    return true;
                }
                return false;
            }
            return base.Equals(obj);
        }
    }
}