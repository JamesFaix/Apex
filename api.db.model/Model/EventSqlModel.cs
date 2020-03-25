﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("Events")]
    public class EventSqlModel
    {
        [Required]
        public int Id { get; set; }
        
        [Required]
        public GameSqlModel Game { get; set; }
        
        [Required]
        public UserSqlModel CreatedByUser { get; set; }
        
        // Nullable
        public PlayerSqlModel ActingPlayer { get; set; }

        [Required]
        public DateTime CreatedOn { get; set; }
        
        [Required]
        public EventKindSqlModel Kind { get; set; }

        [Required]
        public string EffectsJson { get; set; }
    }
}
