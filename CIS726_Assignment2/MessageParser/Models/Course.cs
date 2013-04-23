﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using System.Runtime.Serialization;

namespace MessageParser.Models
{
    /// <summary>
    /// This model class represents a single course in the course catalog
    /// 
    /// Variable credit hour classes can have a min and a max hours, or just
    /// checkmark the Variable flag if the number of hours is unknown
    /// 
    /// It has a list of degree programs it is a part of and a list of elective lists 
    /// it is contained in
    /// </summary>
    public class Course : IModel
    {
        [ScaffoldColumn(false)]
        public override int ID { get; set; }
        
        [DisplayName("Prefix")]
        [Required(ErrorMessage="A Course Prefix is required (eg. CIS)")]
        [StringLength(5, ErrorMessage="The Course Prefix must be 5 characters or less")]
        public String coursePrefix { get; set; }

        [DisplayName("Number")]
        [Required(ErrorMessage="A Course Number is required (eg. 101)")]
        [Range(0, 999, ErrorMessage="Course numbers must be 3 digits or less")]
        [DisplayFormat(DataFormatString="{0:000}")]
        public int courseNumber { get; set; }

        [DisplayName("Course Title")]
        [Required(ErrorMessage="A Course Title is required")]
        public String courseTitle { get; set; }

        [DisplayName("Description")]
        [DataType(DataType.MultilineText)]
        [Required(ErrorMessage="A Course Description is required")]
        public String courseDescription { get; set; }

        [DisplayName("Minimum Credit Hours")]
        [Range(0, 18, ErrorMessage = "The minimum number of Credit Hours must be between 0 and 18 inclusive.")]
        [Required(ErrorMessage = "The minimum number of Credit Hours is required")]
        public int minHours { get; set; }

        [DisplayName("Maximum Credit Hours")]
        [Range(0, 18, ErrorMessage = "The maximum number of Credit Hours must be between 0 and 18 inclusive.")]
        [Required(ErrorMessage = "The maximum number of Credit Hours is required")]
        public int maxHours { get; set; }

        [DisplayName("Undergraduate Class")]
        public Boolean undergrad { get; set; }

        [DisplayName("Graduate Class")]
        public Boolean graduate { get; set; }

        [DisplayName("Variable Credit Hours")]
        public Boolean variable { get; set; }

        /// <summary>
        /// This displays the course header in the format of "CIS 115 - Introduction to Computing Science"
        /// </summary>
        [DisplayName("Course Header")]
        public String courseHeader
        {
            get
            {
                return coursePrefix + " " + String.Format("{0:000}", courseNumber) + " - " + courseTitle;
            }
        }

        /// <summary>
        /// This displays the course catalog number in the format of "CIS 115"
        /// </summary>
        [DisplayName("Course Catalog Number")]
        public String courseCatalogNumber
        {
            get
            {
                return coursePrefix + " " + String.Format("{0:000}", courseNumber);
            }
        }

        /// <summary>
        /// This displays the number of credit hours:
        /// 
        /// If the variable option is true, then display "Variable",
        /// If not, then if min != max, show min-max, as in "3-6"
        /// If not, then just show min, as in "3"
        /// </summary>
        [DisplayName("Credit Hours")]
        public String courseHours
        {
            get
            {
                if (variable)
                {
                    return "Variable";
                }
                else
                {
                    if (minHours == maxHours)
                    {
                        return minHours + "";
                    }
                    else
                    {
                        return minHours + "-" + maxHours;
                    }
                }
            }
        }

        [DisplayName("Credit Hours")]
        public String shortHours
        {
            get
            {
                if (variable)
                {
                    return "Var.";
                }
                else
                {
                    if (minHours == maxHours)
                    {
                        return minHours + "";
                    }
                    else
                    {
                        return minHours + "-" + maxHours;
                    }
                }
            }
        }

        public virtual ICollection<ElectiveListCourse> electiveLists { get; set; }
        public virtual ICollection<RequiredCourse> degreePrograms { get; set; }

        public virtual ICollection<PrerequisiteCourse> prerequisites { get; set; }
        //[IgnoreDataMember] TODO: Unsure if Needed
        public virtual ICollection<PrerequisiteCourse> prerequisiteFor { get; set; }
    }
}