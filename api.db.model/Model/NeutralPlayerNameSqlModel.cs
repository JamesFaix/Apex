﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Apex.Api.Db.Model
{
    [Table("NeutralPlayerNames")]
    public class NeutralPlayerNameSqlModel
    {
        [Required]
        [Column("NeutralPlayerNameId")]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Name { get; set; }
    }
}