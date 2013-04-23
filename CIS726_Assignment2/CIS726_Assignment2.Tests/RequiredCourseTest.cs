﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using MessageParser.Models;

namespace CIS726_Assignment2.Tests
{
    [TestClass]
    public class RequiredCourseTest
    {
        [TestMethod]
        public void RequiredCourseCourseIDisRequired()
        {
            UnitTestHelpers.testIsRequired(typeof(RequiredCourse).GetProperty("courseID"));
        }

        [TestMethod]
        public void RequiredCourseDegreeProgramIDisRequired()
        {
            UnitTestHelpers.testIsRequired(typeof(RequiredCourse).GetProperty("degreeProgramID"));
        }

        [TestMethod]
        public void RequiredCourseSemesterisRequired()
        {
            UnitTestHelpers.testIsRequired(typeof(RequiredCourse).GetProperty("semester"));
        }

        [TestMethod]
        public void RequiredCourseSemesterRange()
        {
            UnitTestHelpers.testRange(typeof(RequiredCourse).GetProperty("semester"), 1, 8);
        }

        [TestMethod]
        public void RequiredCourseEquality()
        {
            RequiredCourse course1 = new RequiredCourse()
            {
                ID = 1,
                courseID = 1
            };
            RequiredCourse course2 = new RequiredCourse()
            {
                ID = 1,
                courseID = 2
            };
            Assert.AreEqual(course1, course2);
        }
    }
}
