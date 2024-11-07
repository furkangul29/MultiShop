﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiShop.Catalog.Dtos.CategoryDtos
{
    public class ResultCategoryWithProductCountDto
    {
        public string CategoryID { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; }
        public int ProductCount { get; set; }
    }
}