﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using MessageParser.Models;

namespace MessageParser.Repositories
{
    /// <summary>
    /// This is the interface for the Storage Contexts used by the Repository system. In a nutshell, this adds
    /// and abstraction layer to the database context that allows injection of other storage containers for the 
    /// purpose of unit testing.
    /// 
    /// Using tutorial from: http://www.tugberkugurlu.com/archive/generic-repository-pattern-entity-framework-asp-net-mvc-and-unit-testing-triangle
    /// </summary>
    /// <typeparam name="T">The class type this will be storing</typeparam>
    public interface IStorageContext<T> where T : IModel
    {
        IQueryable<T> Set();
        T FindByID(int id);
        void Add(T entity);
        void Remove(T entity);
        void Edit(T entity);
        void UpdateValues(T entity, T input);
        void SaveChanges();
        void Dispose();
    }
}