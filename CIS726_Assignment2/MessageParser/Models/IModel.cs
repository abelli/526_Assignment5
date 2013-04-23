﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace MessageParser.Models
{
    /// <summary>
    /// This is the base class for all model classes. It is needed for the Repository framework to function correctly.
    /// 
    /// All it does is expose the ID field at a higher level so the GenericRepository can access it, regardless of the model class
    /// that it is storing.
    /// 
    /// This ID field MUST be the primary key of your model class in the database for everything to function correctly.
    /// </summary>
    /// 
    [DataContract(IsReference=true)]
    public abstract class IModel
    {
        [DataMember]
        public abstract int ID { get; set; }
    }
}