﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services.ApiModels.BookingOffline
{
    public class BookingOfflineRequest
    {
        [Required]
        public string Description { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

    }
}
