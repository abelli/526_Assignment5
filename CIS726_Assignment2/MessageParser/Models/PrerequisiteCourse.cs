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
    public class PrerequisiteCourse : IModel
    {
        [ScaffoldColumn(false)]
        public override int ID { get; set; }

        [Required(ErrorMessage = "A Course ID is required")]
        public int prerequisiteCourseID { get; set; }

        [Required(ErrorMessage = "A Course ID is required")]
        public int prerequisiteForCourseID { get; set; }

        [ForeignKey("prerequisiteCourseID")]
        public virtual Course prerequisiteCourse { get; set; }

        [ForeignKey("prerequisiteForCourseID")]
        public virtual Course prerequisiteForCourse { get; set; }

        public override bool Equals(object obj)
        {
            PrerequisiteCourse course = obj as PrerequisiteCourse;
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