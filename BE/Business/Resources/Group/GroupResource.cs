﻿using Business.Extensions;
using Business.Resources.Technology;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Business.Resources.Group
{
    public class GroupResource
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MaxLength(250)]
        public string Name { get; set; }

        [Required]
        [MaxLength(250)]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Team Size")]
        public int TeamSize { get; set; }

        [Required]
        [JsonConverter(typeof(DateTimeConverter))]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [JsonConverter(typeof(DateTimeConverter))]
        [Display(Name = "End Date")]
        public DateTime? EndDate { get; set; }

        [Required]
        public List<TechnologyResource> Technology { get; set; }
    }
}