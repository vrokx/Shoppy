﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RepoPractice.Models
{
    public class RoleModel
    {
        [Key]
        public int UserTypeId { get; set; }

        [Required]
        public string UserTypeName { get; set; }
    }
}