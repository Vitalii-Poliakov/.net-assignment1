﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Lab4.Models
{
    public class AddImage
    {
        public int AddImageId { get; set; }

        [Required]
        [DisplayName("File Name")]
        public string FileName { get; set; }

        [Required]
        [Url]
        public string Url { get; set; }


    }
}
