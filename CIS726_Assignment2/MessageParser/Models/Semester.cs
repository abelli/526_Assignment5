﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MessageParser.Models
{
    /// <summary>
    /// This class represents a single elective list
    /// 
    /// It has a list of courses that are contained in the list
    /// It also has a list of elective courses that link it to the degree
    /// programs it is a part of
    /// </summary>
    public class Semester : IModel
    {
        [ScaffoldColumn(false)]
        public override int ID { get; set; }

        [DisplayName("Semester Title")]
        [Required(ErrorMessage="A Semester Title is Required")]
        public String semesterTitle { get; set; }

        [DisplayName("Semester Year")]
        [Required(ErrorMessage = "A Semester Year is Required")]
        public int semesterYear { get; set; }

        [DisplayName("Standard Semester")]
        public Boolean standard { get; set; }

        [DisplayName("Semester Name")]
        public String semesterName
        {
            get
            {
                return semesterTitle + " " + semesterYear;
            }
        }
    }
}