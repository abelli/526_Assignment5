﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.ComponentModel.DataAnnotations;
using MessageParser.Models;

namespace CIS726_Assignment2.Tests
{
    [TestClass]
    public class ElectiveCourseTest
    {
        [TestMethod]
        public void ElectiveCourseElectiveListIDIsRequired()
        {
            UnitTestHelpers.testIsRequired(typeof(ElectiveCourse).GetProperty("electiveListID"));
        }

        [TestMethod]
        public void ElectiveCourseDegreeProgramIDIsRequired()
        {
            UnitTestHelpers.testIsRequired(typeof(ElectiveCourse).GetProperty("degreeProgramID"));
        }

        [TestMethod]
        public void ElectiveCourseSemesterIsRequired()
        {
            UnitTestHelpers.testIsRequired(typeof(ElectiveCourse).GetProperty("semester"));
        }

        [TestMethod]
        public void ElectiveCourseSemesterRange()
        {
            UnitTestHelpers.testRange(typeof(ElectiveCourse).GetProperty("semester"), 1, 8);
        }

        [TestMethod]
        public void ElectiveCourseCreditsRequired()
        {
            UnitTestHelpers.testIsRequired(typeof(ElectiveCourse).GetProperty("credits"));
        }

        [TestMethod]
        public void ElectiveCourseCreditsRange()
        {
            UnitTestHelpers.testRange(typeof(ElectiveCourse).GetProperty("credits"), 1, 18);
        }

        [TestMethod]
        public void ElectiveCourseEquality()
        {
            ElectiveCourse course1 = new ElectiveCourse()
            {
                ID = 1,
                electiveListID = 1
            };
            ElectiveCourse course2 = new ElectiveCourse()
            {
                ID = 1,
                electiveListID = 2
            };
            Assert.AreEqual(course1, course2);
        }
    }
}
