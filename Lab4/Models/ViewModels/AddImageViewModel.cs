using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.Models.ViewModels
{
    public class AddImageViewModel
    {
        public Community Community { get; set; }
        public IEnumerable<AddImage> AddImages { get; set; }

    }
}
