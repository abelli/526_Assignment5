﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MessageParser.Models;

namespace CIS726_Assignment2.ViewModels
{
    public class PlanCourseEdit
    {
        public int ID { get; set; }
        public int? courseID { get; set; }
        public string courseHeader { get; set; }
        public string electiveListName {get; set;}
        public string notes { get; set; }

        public PlanCourseEdit(PlanCourse pcourse)
        {
            ID = pcourse.ID;
            if(pcourse.courseID != null){
                courseID = (int)pcourse.courseID;
                courseHeader = pcourse.course.courseHeader;
            }else{
                courseID = null;
                courseHeader = null;
            }
            if (pcourse.electiveListID != null)
            {
                electiveListName = pcourse.electiveList.electiveListName;
            }
            else
            {
                electiveListName = null;
            }
            notes = pcourse.notes;
        }
    }
}