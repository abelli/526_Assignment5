﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace MessageParser.Models
{

    /// <summary>
    /// This class is the many-to-many link between the Plan class and the courses
    /// that are part of that plan of study. 
    /// </summary>
    public class PlanCourse : IModel
    {
        [ScaffoldColumn(false)]
        public override int ID { get; set; }

        [DisplayName("Course Notes")]
        public String notes { get; set; }

        [Required(ErrorMessage="A Plan of study is required")]
        public int planID { get; set; }

        public virtual Plan plan { get; set; }

        public int? courseID { get; set; }
        public int? electiveListID { get; set; }

        public virtual Course course { get; set; }
        public virtual ElectiveList electiveList { get; set; }

        [Required(ErrorMessage="A Semester ID is required")]
        public int semesterID { get; set; }

        [DisplayName("Semester Order")]
        [Required(ErrorMessage="A semester order is required")]
        public int order { get; set; }
        
        public virtual Semester semester { get; set; }

        public String credits { get; set; }

        public override bool Equals(object obj)
        {
            PlanCourse course = obj as PlanCourse;
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