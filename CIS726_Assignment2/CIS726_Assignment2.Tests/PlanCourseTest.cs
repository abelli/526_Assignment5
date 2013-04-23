﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CIS726_Assignment2.Tests;
using MessageParser.Models;


namespace CIS726_Assignment2.Tests
{
    [TestClass]
    public class PlanCourseTest
    {
        [TestMethod]
        public void PlanCoursePlanIDIsRequired()
        {
            UnitTestHelpers.testIsRequired(typeof(PlanCourse).GetProperty("planID"));
        }

        [TestMethod]
        public void PlanCourseSemesterIDIsRequired()
        {
            UnitTestHelpers.testIsRequired(typeof(PlanCourse).GetProperty("semesterID"));
        }

        [TestMethod]
        public void PlanCourseOrderIsRequired()
        {
            UnitTestHelpers.testIsRequired(typeof(PlanCourse).GetProperty("order"));
        }
    }
}
