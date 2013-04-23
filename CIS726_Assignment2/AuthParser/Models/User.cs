﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Globalization;
using MessageParser.Models;

namespace AuthParser.Models
{
    [Table("Users")]
    public class User : IModel
    {
        [ScaffoldColumn(false)]
        [Key]
        public override int ID { get; set; }

        [DisplayName("Username")]
        [Required(ErrorMessage = "The Username is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage="The username must be between 3 and 100 characters")]
        public String username { get; set; }

        [Required]
        [Display(Name = "Password")]
        public String password { get; set; }

        [DisplayName("Real Name")]
        [Required(ErrorMessage = "Your real name is required")]
        [StringLength(100, ErrorMessage="The real name field cannot be longer than 100 characters")]
        public String realName { get; set; }
    }
}