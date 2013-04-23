﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CIS726_Assignment2.Tests;
using MessageParser.Models;
using AuthParser.Models;

namespace CIS726_Assignment2.Tests
{
    [TestClass]
    public class UserTest
    {
        [TestMethod]
        public void UsernameIsRequired()
        {
            UnitTestHelpers.testIsRequired(typeof(User).GetProperty("username"));
        }

        [TestMethod]
        public void UsernameLength()
        {
            UnitTestHelpers.testStringLength(typeof(User).GetProperty("username"), 100);
            UnitTestHelpers.testStringLengthMin(typeof(User).GetProperty("username"), 3);
        }

        [TestMethod]
        public void RealNameIsRequired()
        {
            UnitTestHelpers.testIsRequired(typeof(User).GetProperty("realName"));
        }

        [TestMethod]
        public void RealNameLength()
        {
            UnitTestHelpers.testStringLength(typeof(User).GetProperty("realName"), 100);
        }
    }
}
